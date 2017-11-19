using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
