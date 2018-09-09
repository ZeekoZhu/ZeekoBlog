using System;
using System.Collections.Generic;

namespace ZeekoBlog.Core.Models
{
    public class Article
    {
        public int Id { get; set; }
        public BlogUser BlogUser { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public DateTime LastEdited { get; set; }
        /// <summary>
        /// 文章中涉及到的编程语言
        /// </summary>
        public string Languages { get; set; } = string.Empty;
        /// <summary>
        /// 文章类型，为了兼容以前的文章，默认为 Markdown
        /// </summary>
        public ArticleDocType DocType { get; set; } = ArticleDocType.Markdown;

        public string RenderedContent { get; set; } = string.Empty;
        public string RenderedSummary { get; set; } = string.Empty;
        public List<TOCItem> TOCList { get; set; } = new List<TOCItem>();
        public DateTime Created { get; set; }
        public bool SoftDelete { get; set; } = false;
    }

    public enum ArticleDocType
    {
        Markdown, AsciiDoc, Raw
    }
}
