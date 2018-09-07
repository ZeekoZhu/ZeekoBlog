using System;
using EasyCaching.Core;
using Markdig;
using ZeekoUtilsPack.BCLExt;

namespace ZeekoBlog.Markdown.Plugins
{
    public class HTMLRendererPlugin : BaseMarkdownPlugin
    {
        private readonly MarkdownPipeline _pipeline;
        
        private readonly IEasyCachingProvider _cache;
        public override string Id { get; } = ID;
        public static string ID = "rocks.gianthard.html-renderer";
        public HTMLRendererPlugin(IEasyCachingProvider cache, MarkdownPipeline pipeline = null)
        {
            _cache = cache;
            _pipeline = pipeline;
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
            var result = Markdig.Markdown.ToHtml(output.Source, _pipeline ?? output.Pipeline);
            _cache.Set(key, result, expiration);
            output.Storage.Upsert(key, result);
            output.Html = result;
            return output;
        }
    }
}
