using System;
using System.Collections.Generic;
using System.Text;
using EasyCaching.Core;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using ZeekoUtilsPack.BCLExt;

namespace ZeekoBlog.Markdown.Plugins
{
    public class HTMLRendererPlugin : MarkdownPlugin
    {
        private readonly MarkdownPipeline _pipeline;

        public static readonly MarkdownPipeline DefaultPipeline =
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
        private readonly IEasyCachingProvider _cache;
        public override string Id { get; } = ID;
        public static string ID = "rocks.gianthard.html-renderer";
        public HTMLRendererPlugin(IEasyCachingProvider cache, MarkdownPipeline pipeline = null)
        {
            _cache = cache;
            _pipeline = pipeline ?? DefaultPipeline;
        }
        public override MarkdownOutput Invoke(MarkdownOutput output)
        {
            var key = Id + output.Source.GetMd5();
            var expiration = TimeSpan.FromHours(6);
            var cachedHtml = _cache.Get<string>(key);
            if (cachedHtml.HasValue)
            {
                _cache.Refresh(key, cachedHtml.Value, expiration);
                output.Storage.Upsert(Id, cachedHtml.Value);
                output.Html = cachedHtml.Value;
                return output;
            }
            var result = Markdig.Markdown.ToHtml(output.Source, _pipeline);
            _cache.Set(key, result, expiration);
            output.Storage.Upsert(key, result);
            output.Html = result;
            return output;
        }
    }
}
