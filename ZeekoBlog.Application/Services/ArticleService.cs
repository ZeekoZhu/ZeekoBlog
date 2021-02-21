using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;
using Mapster;
using ZeekoBlog.Application.DTO;
using ZeekoBlog.Core;
using ZeekoBlog.Core.Models;
using TOCItem = ZeekoBlog.Core.Models.TOCItem;

namespace ZeekoBlog.Application.Services
{
    public class ArticleService
    {
        private readonly BlogContext _context;
        private readonly IArticleRenderer _renderer;

        public ArticleService(BlogContext context, IArticleRenderer renderer)
        {
            _context = context;
            _renderer = renderer;
        }

        public async Task<PagedList<ArticleListDto>> GetPaged(int index, int pageSize, int userId)
        {
            var results = await _context.ValidArticles
                .Where(a => a.BlogUser.Id == userId)
                .OrderByDescending(a => a.Created)
                .Skip(index * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var totalCount = await _context.ValidArticles.CountAsync();
            var list = new PagedList<ArticleListDto>
            {
                List = results.Adapt<List<ArticleListDto>>(),
                TotalPage = totalCount / pageSize + (totalCount % pageSize > 0 ? 1 : 0)
            };
            return list;
        }

        public async Task<List<Article>> GetRecentAsync(int count, int userId)
        {
            return await _context.ValidArticles
                .Where(a => a.BlogUser.Id == userId)
                .OrderByDescending(a => a.LastEdited)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Article> GetById(int id)
        {
            var result = await _context.ValidArticles.Include(a => a.TOCList).FirstOrDefaultAsync(a => a.Id == id);
            if (result is null)
                return null;
            result.TOCList = result.TOCList.OrderBy(t => t.Order).ToList();
            return result;
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
            await _renderer.RenderAsync(article);
            _context.Articles.Add(article);
            await _context.SaveChangesAsync();
            article.BlogUser = null;
            return article;
        }

        public async Task<BooleanResult<Article>> UpdateAsync(Article article, int userId)
        {
            var canModify = await _context.ValidArticles.AnyAsync(a => a.Id == article.Id && a.BlogUser.Id == userId);
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
            await _renderer.RenderAsync(article);
            await _context.SaveChangesAsync();
            return new BooleanResult<Article>(true, article);
        }

        /// <summary>
        /// 重新渲染文章内容
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<BooleanResult<Article>> RerenderAsync(int articleId, int userId)
        {
            var target = await _context.ValidArticles.FirstOrDefaultAsync(a => a.Id == articleId && a.BlogUser.Id == userId);
            if (target != null)
            {
                await _context.Set<TOCItem>()
                    .Where(i => i.ArticleId == target.Id)
                    .DeleteAsync();
                await _renderer.RenderAsync(target);
                await _context.SaveChangesAsync();
                return new BooleanResult<Article>(true, target);
            }

            return new BooleanResult<Article>(false, "Article not found");
        }

        // private async Task RenderArticle(Article article)
        // {
        //     switch (article.DocType)
        //     {
        //         case ArticleDocType.AsciiDoc:
        //             var summaryResult = await _asciiDocSvc.Process(article.Summary);
        //             var contentResult = await _asciiDocSvc.Process(article.Content);
        //             article.RenderedSummary = summaryResult.Value;
        //             article.RenderedContent = contentResult.Value;
        //             article.Languages = string.Join(",", contentResult.Languages);
        //             article.TOCList = contentResult.TableOfContents.ToList();
        //             break;
        //         case ArticleDocType.Markdown:
        //             var summary = await _mdSvc.Process(article.Summary);
        //             article.RenderedSummary = summary.Html;
        //             var content = await _mdSvc.Process(article.Content);
        //             article.RenderedContent = content.Html;
        //             var (_, languages) = content.Storage.TryGet<List<string>>(CodeLangDetectionPlugin.ID);
        //             article.Languages = string.Join(",", languages);
        //             (_, article.TOCList) = content.Storage.TryGet<List<TOCItem>>(TOCItemsPlugin.ID);
        //             break;
        //         case ArticleDocType.Raw:
        //             article.RenderedSummary = article.Summary;
        //             article.RenderedContent = article.Content;
        //             break;
        //         default:
        //             throw new ArgumentOutOfRangeException();
        //     }
        // }
    }
}
