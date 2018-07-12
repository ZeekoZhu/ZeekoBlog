using System;
using System.Collections.Generic;
using System.Linq;
using EasyCaching.Core;
using Markdig.Syntax;
using ZeekoUtilsPack.BCLExt;

namespace ZeekoBlog.Markdown.Plugins.CodeLangDetectionPlugin
{
    public class CodeLangDetectionPlugin : MarkdownPlugin<List<string>>
    {
        // console.log(Array.from(temp0.querySelectorAll('span.library-url')).filter(x => x.innerText.endsWith('js')).map(x => '"' + /^.*\/(.*)?\.min\.js$/.exec(x.innerText)[1]+'"').join(','))
        private readonly string[] _languages = {
                "highlight",
                "1c",
                "abnf",
                "accesslog",
                "actionscript",
                "ada",
                "apache",
                "applescript",
                "arduino",
                "armasm",
                "asciidoc",
                "aspectj",
                "autohotkey",
                "autoit",
                "avrasm",
                "awk",
                "axapta",
                "bash",
                "basic",
                "bnf",
                "brainfuck",
                "cal",
                "capnproto",
                "ceylon",
                "clean",
                "clojure-repl",
                "clojure",
                "cmake",
                "coffeescript",
                "coq",
                "cos",
                "cpp",
                "crmsh",
                "crystal",
                "cs",
                "csp",
                "css",
                "d",
                "dart",
                "delphi",
                "diff",
                "django",
                "dns",
                "dockerfile",
                "dos",
                "dsconfig",
                "dts",
                "dust",
                "ebnf",
                "elixir",
                "elm",
                "erb",
                "erlang-repl",
                "erlang",
                "excel",
                "fix",
                "flix",
                "fortran",
                "fsharp",
                "gams",
                "gauss",
                "gcode",
                "gherkin",
                "glsl",
                "go",
                "golo",
                "gradle",
                "groovy",
                "haml",
                "handlebars",
                "haskell",
                "haxe",
                "hsp",
                "htmlbars",
                "http",
                "hy",
                "inform7",
                "ini",
                "irpf90",
                "java",
                "javascript",
                "jboss-cli",
                "json",
                "julia-repl",
                "julia",
                "kotlin",
                "lasso",
                "ldif",
                "leaf",
                "less",
                "lisp",
                "livecodeserver",
                "livescript",
                "llvm",
                "lsl",
                "lua",
                "makefile",
                "markdown",
                "mathematica",
                "matlab",
                "maxima",
                "mel",
                "mercury",
                "mipsasm",
                "mizar",
                "mojolicious",
                "monkey",
                "moonscript",
                "n1ql",
                "nginx",
                "nimrod",
                "nix",
                "nsis",
                "objectivec",
                "ocaml",
                "openscad",
                "oxygene",
                "parser3",
                "perl",
                "pf",
                "php",
                "pony",
                "powershell",
                "processing",
                "profile",
                "prolog",
                "protobuf",
                "puppet",
                "purebasic",
                "python",
                "q",
                "qml",
                "r",
                "rib",
                "roboconf",
                "routeros",
                "rsl",
                "ruby",
                "ruleslanguage",
                "rust",
                "scala",
                "scheme",
                "scilab",
                "scss",
                "shell",
                "smali",
                "smalltalk",
                "sml",
                "sqf",
                "sql",
                "stan",
                "stata",
                "step21",
                "stylus",
                "subunit",
                "swift",
                "taggerscript",
                "tap",
                "tcl",
                "tex",
                "thrift",
                "tp",
                "twig",
                "typescript",
                "vala",
                "vbnet",
                "vbscript-html",
                "vbscript",
                "verilog",
                "vhdl",
                "vim",
                "x86asm",
                "xl",
                "xml",
                "xquery",
                "yaml",
                "zephir"
            };

        private readonly Dictionary<string, string> _languageShort = new Dictionary<string, string>
        {
            ["ts"] = "typescript",
            ["js"] = "javascript",
            ["posh"] = "powershell",
            ["fs"] = "fsharp",
            ["sh"] = "shell",
        };
        private readonly IEasyCachingProvider _cache;
        public static readonly string ID = "rocks.gianthard.code-lang";
        public override string Id { get; } = ID;

        public CodeLangDetectionPlugin(IEasyCachingProvider cache)
        {
            _cache = cache;
        }

        public override MarkdownOutput Invoke(MarkdownOutput output)
        {
            var key = Id + output.Source.GetMd5();
            var expiration = TimeSpan.FromHours(6);
            var cached = _cache.Get<List<string>>(key);

            if (cached.HasValue)
            {
                _cache.Refresh(key, cached.Value, expiration);
                output.Storage.Upsert(Id, cached.Value);
                return output;
            }

            var doc = output.Document;
            var languages = doc.Descendants<FencedCodeBlock>()
                .Select(c => c.Info).Where(l => l != null)
                .Distinct()
                .Where(l => _languages.Contains(l) || _languageShort.ContainsKey(l))
                .Select(l => _languageShort.TryGetValue(l, out string result) ? result : l)
                .ToList();
            output.Storage.Upsert(Id, languages);
            _cache.Set(key, languages, expiration);
            return output;
        }
    }
}
