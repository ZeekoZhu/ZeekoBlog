using Microsoft.EntityFrameworkCore;

namespace ZeekoBlog.Models
{
    public class BlogContext : DbContext
    {
        public DbSet<Article> Articles { get; set; }

        public BlogContext(DbContextOptions<BlogContext> options) : base(options)
        { }
    }
}
