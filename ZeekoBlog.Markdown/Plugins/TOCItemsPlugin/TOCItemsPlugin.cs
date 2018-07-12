using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EasyCaching.Core;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using ZeekoUtilsPack.BCLExt;

namespace ZeekoBlog.Markdown.Plugins.TOCItemsPlugin
{
    public class TOCItemsPlugin : MarkdownPlugin<List<TOCItem>>
    {
        private readonly IEasyCachingProvider _cache;
        public static readonly string ID = "rocks.gianthard.toc-items";
        public override string Id { get; } = ID;

        public TOCItemsPlugin(IEasyCachingProvider cache)
        {
            _cache = cache;
        }
        public override MarkdownOutput Invoke(MarkdownOutput output)
        {
            var key = Id + output.Source.GetMd5();
            var expiration = TimeSpan.FromHours(6);
            var cached = _cache.Get<List<TOCItem>>(key);

            if (cached.HasValue)
            {
                _cache.Refresh(key, cached.Value, expiration);
                output.Storage.Upsert(Id, cached.Value);
                return output;
            }

            var doc = output.Document;
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
            output.Storage.Upsert(Id, items);
            _cache.Set(key, items, TimeSpan.FromDays(1));
            return output;
        }
    }
}
