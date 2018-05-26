using System.Collections.Generic;
using System.IO;
using System.Linq;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using ZeekoBlog.ViewModels;

namespace ZeekoBlog.Services
{
    public class MarkdownService
    {

        private static readonly MarkdownPipeline RenderPipeline =
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

        private static readonly MarkdownPipeline TOCPipline = new MarkdownPipelineBuilder()
            .UseAutoIdentifiers(AutoIdentifierOptions.AutoLink).Build();

        /// <summary>
        /// 获取文档中用到的所有的编程语言列表
        /// </summary>
        /// <param name="mdContent"></param>
        /// <returns></returns>
        public List<string> GetLanguages(string mdContent)
        {
            var doc = Markdown.Parse(mdContent);
            var languages = doc.Descendants<FencedCodeBlock>()
                .Select(c => c.Info).Where(l => l != null).Distinct().ToList();
            return languages;
        }

        /// <summary>
        /// 渲染文档为 HTML
        /// </summary>
        /// <param name="mdContent"></param>
        /// <returns></returns>
        public string Render(string mdContent)
        {
            return Markdown.ToHtml(mdContent, RenderPipeline);
        }

        public List<TOCItem> GetTableOfContent(string mdContent)
        {
            var doc = Markdown.Parse(mdContent, TOCPipline);
            var items = doc.Descendants<HeadingBlock>().Select(block =>
            {
                var sw = new StringWriter();
                var htmlWriter = new HtmlRenderer(sw) { EnableHtmlForInline = false };
                htmlWriter.WriteLeafInline(block);
                var name = sw.ToString();
                return new TOCItem
                {
                    Level = block.Level,
                    Name = name,
                    Id = block.GetAttributes().Id
                };
            });
            return items.ToList();
        }
    }
}
