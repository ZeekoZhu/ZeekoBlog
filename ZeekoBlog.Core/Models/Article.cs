using System;

namespace ZeekoBlog.Core.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public DateTime LastEdited { get; set; }
    }
}
