using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Markdig.Syntax;

namespace ZeekoBlog.Markdown
{
    public class MarkdownOutput
    {
        public string Source { get; set; }
        public string Html { get; set; }
        public MarkdownPipeline Pipeline { get; set; } =
            new MarkdownPipelineBuilder()
                .UseAbbreviations()
                .UseAutoIdentifiers(AutoIdentifierOptions.AutoLink)
                .UseCustomContainers()
                .UseDefinitionLists()
                .UseFootnotes()
                .UseGridTables()
                .UseMediaLinks()
                .UsePipeTables()
                .UseListExtras()
                .UseTaskLists()
                .UseAutoLinks()
                .UseGenericAttributes()
                .Build();
        public MarkdownDocument Document { get; set; }
        public PluginStorage Storage { get; set; }

    }
}
