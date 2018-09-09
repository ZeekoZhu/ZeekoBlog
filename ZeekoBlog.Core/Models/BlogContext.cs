using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ZeekoBlog.Core.Models
{
    public class BlogContext : DbContext
    {
        public DbSet<Article> Articles { get; set; }

        public IQueryable<Article> ValidArticles => Articles.Where(a => a.SoftDelete == false);

        public BlogContext(DbContextOptions<BlogContext> options) : base(options)
        { }
    }
}
