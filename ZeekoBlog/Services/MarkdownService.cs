using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Microsoft.Extensions.Caching.Memory;
using ZeekoBlog.ViewModels;
using ZeekoUtilsPack.BCLExt;

namespace ZeekoBlog.Services
{
    public class MarkdownService
    {
        private readonly IMemoryCache _cache;

        public MarkdownService(IMemoryCache cache)
        {
            _cache = cache;
        }
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
            var key = "lang" + mdContent.GetMd5();
            if (_cache.TryGetValue(key, out List<string> cached))
            {
                return cached;
            }
            var doc = Markdown.Parse(mdContent);
            var languages = doc.Descendants<FencedCodeBlock>()
                .Select(c => c.Info).Where(l => l != null).Distinct().ToList();
            _cache.Set(key, languages, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromDays(1)
            });
            return languages;
        }

        /// <summary>
        /// 渲染文档为 HTML
        /// </summary>
        /// <param name="mdContent"></param>
        /// <returns></returns>
        public string Render(string mdContent)
        {
            var key = "render" + mdContent.GetMd5();
            if (_cache.TryGetValue(key, out string cached))
            {
                return cached;
            }
            var result = Markdown.ToHtml(mdContent, RenderPipeline);
            _cache.Set(key, result, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromDays(1)
            });
            return result;
        }

        /// <summary>
        /// 获取文档的 TOC 列表
        /// </summary>
        /// <param name="mdContent"></param>
        /// <returns></returns>
        public List<TOCItem> GetTableOfContent(string mdContent)
        {
            var key = "toc" + mdContent.GetMd5();
            if (_cache.TryGetValue(key, out List<TOCItem> cached))
            {
                return cached;
            }
            var doc = Markdown.Parse(mdContent, TOCPipline);
            var sw = new StringWriter();
            var htmlWriter = new HtmlRenderer(sw) { EnableHtmlForInline = false };
            var items = doc.Descendants<HeadingBlock>().Select(block =>
            {
                htmlWriter.WriteLeafInline(block);
                var name = sw.ToString();
                sw.GetStringBuilder().Clear();
                return new TOCItem
                {
                    Level = block.Level,
                    Name = name,
                    Id = block.GetAttributes().Id
                };
            }).ToList();
            _cache.Set(key, items, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromDays(1)
            });
            return items.ToList();
        }
    }
}
