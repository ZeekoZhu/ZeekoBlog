using System;
using System.Threading.Tasks;
using ZeekoBlog.CodeHighlight;

namespace ZeekoBlog.Markdown.Plugins.HLJSPlugin
{
    public class HLJSPlugin : BaseMarkdownPlugin
    {
        private readonly CodeHighlightService _hljsSvc;

        public HLJSPlugin(CodeHighlightService hljsSvc)
        {
            _hljsSvc = hljsSvc;
        }
        public override string Id { get; }
        public override Task<MarkdownOutput> InvokeAsync(MarkdownOutput output)
        {
            //output.Pipeline
            throw new NotImplementedException();
        }
    }
}
