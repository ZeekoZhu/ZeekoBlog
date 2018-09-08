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
        public override MarkdownOutput Invoke(MarkdownOutput output)
        {
            //output.Pipeline
            output.Pipeline.Extensions.AddIfNotAlready(new HLJSExtension(_hljsSvc));
            return output;
        }
    }
}
