using System;

namespace ZeekoBlog.DTOs
{
    public class ArticleListDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public DateTime LastEdited { get; set; }
    }

    public class ArticleDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public DateTime LastEdited { get; set; }
    }

    public class ArticlePostDto
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
    }
}
