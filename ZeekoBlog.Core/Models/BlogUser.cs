using System.Collections.Generic;

namespace ZeekoBlog.Core.Models
{
    public class BlogUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public List<Article> Articles { get; set; }
    }
}
