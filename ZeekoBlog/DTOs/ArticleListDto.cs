using System;
using System.ComponentModel.DataAnnotations;
using ZeekoBlog.Core.Models;

namespace ZeekoBlog.DTOs
{
    public class ArticleListDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public DateTime LastEdited { get; set; }
        public ArticleDocType DocType { get; set; }
    }

    public class ArticleDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public DateTime LastEdited { get; set; }
        public ArticleDocType DocType { get; set; }
    }

    public class ArticlePostDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Summary { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public ArticleDocType DocType { get; set; }
    }
}
