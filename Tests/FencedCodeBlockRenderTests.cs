using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Markdig;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using ZeekoBlog.CodeHighlight;
using ZeekoBlog.Markdown;
using ZeekoBlog.Markdown.Plugins.HLJSPlugin;

namespace Tests
{
    public class FencedCodeBlockRenderTests
    {
        readonly MarkdownPipeline _pipeline;
        public FencedCodeBlockRenderTests()
        {
            var services = new ServiceCollection();
            services.AddCodeHighlight(new[] { "summary" });
            var hlSvc = services.BuildServiceProvider().GetService<CodeHighlightService>();
            var output = new MarkdownOutput();
            _pipeline = output.Pipeline;
            _pipeline.Extensions.Add(new HLJSExtension(hlSvc));

        }

        void AssertFencedCode(string html, string lang)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var pre = doc.DocumentNode.QuerySelector("pre.hljs");
            Assert.NotNull(pre);
            var code = pre.QuerySelector("code");
            Assert.True(code.HasClass("language-" + lang));
        }

        void AssertIndentedCode(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var pre = doc.DocumentNode.QuerySelector("pre.hljs");
            Assert.Null(pre);
        }

        [Fact]
        public void FencedCodeBlockShouldBeRendered()
        {
            var code = @"
```fsharp
module internal Utils

module TryParse =
    let tryParseWith tryParseFunc =
        tryParseFunc >> function
            | true, v    -> Some v
            | false, _   -> None

    let parseDate   = tryParseWith System.DateTime.TryParse
    let parseInt    = tryParseWith System.Int32.TryParse
    let parseSingle = tryParseWith System.Single.TryParse
    let parseDouble = tryParseWith System.Double.TryParse
    // etc.

    // active patterns for try-parsing strings
    let (|Date|_|)   = parseDate
    let (|Int|_|)    = parseInt
    let (|Single|_|) = parseSingle
    let (|Double|_|) = parseDouble
";
            var result = Markdown.ToHtml(code, _pipeline);
            Assert.NotEqual(result, code);
            Assert.False(string.IsNullOrWhiteSpace(result));
            AssertFencedCode(result, "fsharp");
        }

        [Fact]
        public void ShouldNotRenderIndentedCodeBlock()
        {
            var code = @"
        hello, there
";
            var result = Markdown.ToHtml(code, _pipeline);
            Assert.NotEqual(code, result);
            AssertIndentedCode(result);
        }

        [Fact]
        public void ShouldRenderFencedCodeWithoutLang()
        {
            var code = @"
```
module internal Utils

module TryParse =
    let tryParseWith tryParseFunc =
        tryParseFunc >> function
            | true, v    -> Some v
            | false, _   -> None

    let parseDate   = tryParseWith System.DateTime.TryParse
    let parseInt    = tryParseWith System.Int32.TryParse
    let parseSingle = tryParseWith System.Single.TryParse
    let parseDouble = tryParseWith System.Double.TryParse
    // etc.

    // active patterns for try-parsing strings
    let (|Date|_|)   = parseDate
    let (|Int|_|)    = parseInt
    let (|Single|_|) = parseSingle
    let (|Double|_|) = parseDouble
";
            var result = Markdown.ToHtml(code, _pipeline);
            Assert.NotEqual(result, code);
            Assert.False(string.IsNullOrWhiteSpace(result));
            AssertFencedCode(result, "coq");    // 这段 F# 代码会被认为是 coq
        }

        [Fact]
        public void ShouldNotHighlightSummary()
        {
            var code = @"
```summary
一段摘要
```";
            var result = Markdown.ToHtml(code, _pipeline);
            Assert.NotEqual(code, result);
            AssertIndentedCode(result);
        }
    }
}
