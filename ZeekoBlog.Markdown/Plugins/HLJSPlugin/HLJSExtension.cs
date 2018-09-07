using System;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using ZeekoBlog.CodeHighlight;

namespace ZeekoBlog.Markdown.Plugins.HLJSPlugin
{
    public class HLJSExtension : IMarkdownExtension
    {
        private readonly CodeHighlightService _hlSvc;

        public HLJSExtension(CodeHighlightService hlSvc)
        {
            _hlSvc = hlSvc;
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer == null)
            {
                throw new ArgumentNullException(nameof(renderer));
            }

            var htmlRenderer = renderer as TextRendererBase<HtmlRenderer>;
            if (htmlRenderer == null)
            {
                return;
            }

            var originalCodeBlockRenderer = htmlRenderer.ObjectRenderers.FindExact<CodeBlockRenderer>();
            if (originalCodeBlockRenderer != null)
            {
                htmlRenderer.ObjectRenderers.Remove(originalCodeBlockRenderer);
            }

            htmlRenderer.ObjectRenderers.AddIfNotAlready(
                new HLJSCodeBlockRenderer(_hlSvc, originalCodeBlockRenderer));
        }
    }
}
