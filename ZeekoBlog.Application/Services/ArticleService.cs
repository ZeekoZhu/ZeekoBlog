using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;
using ZeekoBlog.Core;
using ZeekoBlog.Core.Models;
using ZeekoBlog.Markdown;
using ZeekoBlog.Markdown.Plugins.CodeLangDetectionPlugin;
using ZeekoBlog.Markdown.Plugins.TOCItemsPlugin;
using TOCItem = ZeekoBlog.Core.Models.TOCItem;

namespace ZeekoBlog.Application.Services
{
    public class ArticleService
    {
        private readonly BlogContext _context;
        private readonly MarkdownService _mdSvc;

        public ArticleService(BlogContext context, MarkdownService mdSvc)
        {
            _context = context;
            _mdSvc = mdSvc;
        }

        public async Task<(List<Article> Articles, int TotalPages)> GetPaged(int index, int pageSize, int userId)
        {
            var results = await _context.Articles
                .Where(a => a.BlogUser.Id == userId)
                .OrderByDescending(a => a.Id)
                .Skip(index * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var totalCount = await _context.Articles.CountAsync();
            return (results, totalCount / pageSize + (totalCount % pageSize > 0 ? 1 : 0));
        }

        public async Task<Article> GetById(int id)
        {
            return await _context.Articles.Include(a => a.TOCList).FirstOrDefaultAsync(a => a.Id == id);
        }

        /// <summary>
        /// 添加新的文章
        /// </summary>
        /// <param name="article"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Article> AddAsync(Article article, int userId)
        {
            var user = await _context.Set<BlogUser>().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return null;
            }

            article.Created = DateTime.UtcNow;
            article.LastEdited = DateTime.UtcNow;
            article.BlogUser = user;
            await RenderArticle(article);
            _context.Entry(article).State = EntityState.Added;
            await _context.SaveChangesAsync();
            article.BlogUser = null;
            return article;
        }

        public async Task<BooleanResult<Article>> UpdateAsync(Article article, int userId)
        {
            var canModify = await _context.Articles.AnyAsync(a => a.Id == article.Id && a.BlogUser.Id == userId);
            if (canModify == false)
            {
                return new BooleanResult<Article>(false, "Article not found");
            }
            _context.Entry(article).State = EntityState.Modified;
            _context.Entry(article).Property(a => a.Created).IsModified = false;
            await _context.Set<TOCItem>()
                .Where(i => i.ArticleId == article.Id)
                .DeleteAsync();

            article.LastEdited = DateTime.UtcNow;
            await RenderArticle(article);
            await _context.SaveChangesAsync();
            return new BooleanResult<Article>(true, article);
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.Id == id);
        }

        private async Task RenderArticle(Article article)
        {
            switch (article.DocType)
            {
                case ArticleDocType.AsciiDoc:
                    // TODO: Asciidoc
                    article.RenderedSummary = article.Summary;
                    article.RenderedContent = article.Content;
                    break;
                case ArticleDocType.Markdown:
                    var summary = await _mdSvc.Process(article.Summary);
                    article.RenderedSummary = summary.Html;
                    var content = await _mdSvc.Process(article.Content);
                    article.RenderedContent = content.Html;
                    var (_, languages) = content.Storage.TryGet<List<string>>(CodeLangDetectionPlugin.ID);
                    article.Languages = string.Join(",", languages);
                    (_, article.TOCList) = content.Storage.TryGet<List<TOCItem>>(TOCItemsPlugin.ID);
                    break;
                case ArticleDocType.Raw:
                    article.RenderedSummary = article.Summary;
                    article.RenderedContent = article.Content;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
