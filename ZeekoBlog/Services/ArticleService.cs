using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZeekoBlog.Core.Models;

namespace ZeekoBlog.Services
{
    public class ArticleService
    {
        private readonly BlogContext _context;

        public ArticleService(BlogContext context)
        {
            _context = context;
        }

        public async Task<(List<Article> Articles, int TotalPages)> GetPaged(int index, int pageSize)
        {
            var results = await _context.Articles
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
    }
}
