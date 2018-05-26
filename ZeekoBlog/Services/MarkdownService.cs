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
        // console.log(Array.from(temp0.querySelectorAll('span.library-url')).filter(x => x.innerText.endsWith('js')).map(x => '"' + /^.*\/(.*)?\.min\.js$/.exec(x.innerText)[1]+'"').join(','))
        private readonly string[] _languages =
        {
            "highlight", "1c", "abnf", "accesslog", "actionscript", "ada", "apache", "applescript", "arduino", "armasm",
            "asciidoc", "aspectj", "autohotkey", "autoit", "avrasm", "awk", "axapta", "bash", "basic", "bnf",
            "brainfuck", "cal", "capnproto", "ceylon", "clean", "clojure-repl", "clojure", "cmake", "coffeescript",
            "coq", "cos", "cpp", "crmsh", "crystal", "cs", "csp", "css", "d", "dart", "delphi", "diff", "django", "dns",
            "dockerfile", "dos", "dsconfig", "dts", "dust", "ebnf", "elixir", "elm", "erb", "erlang-repl", "erlang",
            "excel", "fix", "flix", "fortran", "fsharp", "gams", "gauss", "gcode", "gherkin", "glsl", "go", "golo",
            "gradle", "groovy", "haml", "handlebars", "haskell", "haxe", "hsp", "htmlbars", "http", "hy", "inform7",
            "ini", "irpf90", "java", "javascript", "jboss-cli", "json", "julia-repl", "julia", "kotlin", "lasso",
            "ldif", "leaf", "less", "lisp", "livecodeserver", "livescript", "llvm", "lsl", "lua", "makefile",
            "markdown", "mathematica", "matlab", "maxima", "mel", "mercury", "mipsasm", "mizar", "mojolicious",
            "monkey", "moonscript", "n1ql", "nginx", "nimrod", "nix", "nsis", "objectivec", "ocaml", "openscad",
            "oxygene", "parser3", "perl", "pf", "php", "pony", "powershell", "processing", "profile", "prolog",
            "protobuf", "puppet", "purebasic", "python", "q", "qml", "r", "rib", "roboconf", "routeros", "rsl", "ruby",
            "ruleslanguage", "rust", "scala", "scheme", "scilab", "scss", "shell", "smali", "smalltalk", "sml", "sqf",
            "sql", "stan", "stata", "step21", "stylus", "subunit", "swift", "taggerscript", "tap", "tcl", "tex",
            "thrift", "tp", "twig", "typescript", "vala", "vbnet", "vbscript-html", "vbscript", "verilog", "vhdl",
            "vim", "x86asm", "xl", "xml", "xquery", "yaml", "zephir"
        };

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
                .Select(c => c.Info).Where(l => l != null).Distinct().Where(l => _languages.Contains(l)).ToList();
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
