using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZeekoBlog.Core.Models;

namespace ZeekoBlog.Core.Services
{
    public class ArticleService
    {
        private readonly BlogContext _context;

        public ArticleService(BlogContext context)
        {
            _context = context;
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
            return await _context.Articles.FirstOrDefaultAsync(a => a.Id == id);
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

            article.BlogUser = user;
            _context.Entry(article).State = EntityState.Added;
            await _context.SaveChangesAsync();
            article.BlogUser = null;
            return article;
        }

        public async Task<BooleanResult<Article>> UpdateAsync(Article article)
        {
            _context.Entry(article).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(article.Id))
                {
                    return new BooleanResult<Article>(false,"Article not found");
                }
                else
                {
                    throw;
                }
            }

            return new BooleanResult<Article>(true, article);
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.Id == id);
        }
    }
}
