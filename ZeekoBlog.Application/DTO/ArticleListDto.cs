using System;
using System.ComponentModel.DataAnnotations;
using ZeekoBlog.Core.Models;

namespace ZeekoBlog.Application.DTO
{
    public class ArticleListDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string RenderedSummary { get; set; }
        public string RenderedContent { get; set; }
        public DateTime LastEdited { get; set; }
        public DateTime Created { get; set; }
        public ArticleDocType DocType { get; set; }
    }

    public class ArticleDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string RenderedSummary { get; set; }
        public string RenderedContent { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public DateTime LastEdited { get; set; }
        public DateTime Created { get; set; }
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
