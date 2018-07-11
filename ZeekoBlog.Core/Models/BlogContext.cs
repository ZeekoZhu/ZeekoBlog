using Microsoft.EntityFrameworkCore;

namespace ZeekoBlog.Core.Models
{
    public class BlogContext : DbContext
    {
        public DbSet<Article> Articles { get; set; }

        public BlogContext(DbContextOptions<BlogContext> options) : base(options)
        { }
    }
}
