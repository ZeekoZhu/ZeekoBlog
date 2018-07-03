using System;
using EasyCaching.Core;
using Markdig;
using Markdig.Syntax;
using ZeekoUtilsPack.BCLExt;

namespace ZeekoBlog.Markdown.Plugins
{
    public class SyntaxParserPlugin:MarkdownPlugin
    {
        private readonly IEasyCachingProvider _cache;
        private readonly MarkdownPipeline _pipeline;

        public SyntaxParserPlugin(IEasyCachingProvider cache, MarkdownPipeline pipeline = null)
        {
            _cache = cache;
            _pipeline = pipeline ?? HTMLRendererPlugin.DefaultPipeline;
        }

        public override string Id { get; } = "rocks.gianthard.parser";
        public override MarkdownOutput Invoke(MarkdownOutput output)
        {
            var key = Id + output.Source.GetMd5();
            var expiration = TimeSpan.FromHours(6);
            var cached = _cache.Get<MarkdownDocument>(key);
            if (cached.HasValue)
            {
                _cache.Refresh(key, cached.Value, expiration);
                output.Storage.Upsert(Id, cached.Value);
                output.Document = cached.Value;
                return output;
            }
            var result = Markdig.Markdown.Parse(output.Source, _pipeline);
            _cache.Set(key, result, expiration);
            output.Storage.Upsert(key, result);
            output.Document = result;
            return output;
        }
    }
}
