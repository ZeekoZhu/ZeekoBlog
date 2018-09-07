using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using ZeekoBlog.CodeHighlight;

namespace Tests
{
    public class CodeHighlightServiceTests
    {
        CodeHighlightService _hlSvc;
        public CodeHighlightServiceTests()
        {
            var services = new ServiceCollection();
            services.AddCodeHighlight();
            _hlSvc = services.BuildServiceProvider().GetService<CodeHighlightService>();
        }
        [Fact]
        public async Task LanguageDetection()
        {
            const string fsCode =
@"
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
            var result = await _hlSvc.HighlightAsync(fsCode, "fsharp");
            Assert.True(result.IsSuccess);
            Assert.NotEqual(result.Result, fsCode);
        }
    }
}
