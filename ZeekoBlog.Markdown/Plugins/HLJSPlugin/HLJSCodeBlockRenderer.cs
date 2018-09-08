using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using ZeekoBlog.CodeHighlight;

namespace ZeekoBlog.Markdown.Plugins.HLJSPlugin
{
    public class HLJSCodeBlockRenderer : HtmlObjectRenderer<CodeBlock>
    {
        private readonly CodeHighlightService _hljsSvc;
        private readonly CodeBlockRenderer _underlyingRenderer;

        public HLJSCodeBlockRenderer(CodeHighlightService hljsSvc, CodeBlockRenderer underlyingRenderer = null)
        {
            _hljsSvc = hljsSvc;
            _underlyingRenderer = underlyingRenderer ?? new CodeBlockRenderer();
        }

        void RenderFencedCodeBlock(HtmlRenderer renderer, FencedCodeBlock block)
        {
            var attributes = block.TryGetAttributes() ?? new HtmlAttributes();

            var codeLines = block.Lines.Lines.Select(l => l.ToString());
            var code = string.Join("\n", codeLines);
            var hlResult = ApplySyntaxHighlighting(code, block.Info);
            if (hlResult.IsSuccess == false)
            {
                // 代码高亮失败或者被跳过，由默认的渲染器进行渲染
                _underlyingRenderer.Write(renderer, block);
                return;
            }
            attributes.AddProperty("data-language", hlResult.Language);
            attributes.AddClass("hljs");
            if (string.IsNullOrWhiteSpace(block.Info))
            {
                attributes.AddClass($"language-{hlResult.Language}");
            }

            renderer
                .Write("<pre>")
                .Write("<code")
                .WriteAttributes(attributes)
                .Write(">");


            renderer.WriteLine(hlResult.Result);
            renderer
                .Write("</code>")
                .WriteLine("</pre>");
        }

        protected override void Write(HtmlRenderer renderer, CodeBlock block)
        {
            switch (block)
            {
                case FencedCodeBlock fenced:
                    RenderFencedCodeBlock(renderer, fenced);
                    break;
                default:
                    _underlyingRenderer.Write(renderer, block);
                    break;
            }
        }

        private HighlightResult ApplySyntaxHighlighting(string code, string lang)
        {
            var highlightTask = _hljsSvc.HighlightAsync(code, lang);
            highlightTask.Wait();
            return highlightTask.Result;
        }
    }
}
