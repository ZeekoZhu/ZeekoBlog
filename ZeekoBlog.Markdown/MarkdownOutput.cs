using Markdig.Syntax;

namespace ZeekoBlog.Markdown
{
    public class MarkdownOutput
    {
        public string Source { get; set; }
        public string Html { get; set; }
        public MarkdownDocument Document { get; set; }
        public PluginStorage Storage { get; set; }

    }
}
