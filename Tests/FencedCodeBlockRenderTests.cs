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
            var pre = doc.DocumentNode.QuerySelector("pre");
            Assert.NotNull(pre);
            var code = pre.QuerySelector("code.hljs");
            Assert.True(code.HasClass("language-" + lang));
        }

        void AssertIndentedCode(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var pre = doc.DocumentNode.QuerySelector("pre code.hljs");
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

        [Fact]
        public void ShouldRenderLongCodeBlock()
        {
            var code = @"
```csharp
/// <summary>
/// user info |> jwt |> store in ticket |> serialize |> data protection |> base64 encode
/// https://amanagrawal.blog/2017/09/18/jwt-token-authentication-with-cookies-in-asp-net-core/
/// </summary>
public class EasyJwtAuthTicketFormat : ISecureDataFormat<AuthenticationTicket>
{
    private readonly TokenValidationParameters _validationParameters;
    private readonly IDataSerializer<AuthenticationTicket> _ticketSerializer;
    private readonly IDataProtector _dataProtector;

    /// <summary>
    /// Create a new instance of the <see cref=""EasyJwtAuthTicketFormat""/>
    /// </summary>
    /// <param name=""validationParameters"">
    /// instance of <see cref=""TokenValidationParameters""/> containing the parameters you
    /// configured for your application
    /// </param>
    /// <param name=""ticketSerializer"">
    /// an implementation of <see cref=""IDataSerializer{TModel}""/>. The default implemenation can
    /// also be passed in""/&gt;
    /// </param>
    /// <param name=""dataProtector"">
    /// an implementation of <see cref=""IDataProtector""/> used to securely encrypt and decrypt
    /// the authentication ticket.
    /// </param>
    public EasyJwtAuthTicketFormat(TokenValidationParameters validationParameters,
        IDataSerializer<AuthenticationTicket> ticketSerializer,
        IDataProtector dataProtector)
    {
        _validationParameters = validationParameters ??
                                    throw new ArgumentNullException($""{nameof(validationParameters)} cannot be null"");
        _ticketSerializer = ticketSerializer ??
                                throw new ArgumentNullException($""{nameof(ticketSerializer)} cannot be null""); ;
        _dataProtector = dataProtector ??
                             throw new ArgumentNullException($""{nameof(dataProtector)} cannot be null"");
    }

    /// <summary>
    /// Does the exact opposite of the Protect methods i.e. converts an encrypted string back to
    /// the original <see cref=""AuthenticationTicket""/> instance containing the JWT and claims.
    /// </summary>
    /// <param name=""protectedText""></param>
    /// <returns></returns>
    public AuthenticationTicket Unprotect(string protectedText)
        => Unprotect(protectedText, null);

    /// <summary>
    /// Does the exact opposite of the Protect methods i.e. converts an encrypted string back to
    /// the original <see cref=""AuthenticationTicket""/> instance containing the JWT and claims.
    /// Additionally, optionally pass in a purpose string.
    /// </summary>
    /// <param name=""protectedText""></param>
    /// <param name=""purpose""></param>
    /// <returns></returns>
    public AuthenticationTicket Unprotect(string protectedText, string purpose)
    {
        var authTicket = _ticketSerializer.Deserialize(
            _dataProtector.Unprotect(
                Base64UrlTextEncoder.Decode(protectedText)));

        var embeddedJwt = authTicket
            .Properties?
            .GetTokenValue(JwtBearerDefaults.AuthenticationScheme);

        try
        {
            // 校验并读取 jwt 中的用户信息（Claims）
            var principal = new JwtSecurityTokenHandler()
                .ValidateToken(embeddedJwt, _validationParameters, out var token);

            if (!(token is JwtSecurityToken))
            {
                throw new SecurityTokenValidationException(""JWT token was found to be invalid"");
            }
            // todo: 此处还可以校验 token 是否被吊销
            // 将 jwt 中的用户信息与 Cookie 中的包含的用户信息合并起来
            authTicket.Principal.AddIdentities(principal.Identities);
            return authTicket;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Protect the authentication ticket and convert it to an encrypted string before sending
    /// out to the users.
    /// </summary>
    /// <param name=""data"">an instance of <see cref=""AuthenticationTicket""/></param>
    /// <returns>encrypted string representing the <see cref=""AuthenticationTicket""/></returns>
    public string Protect(AuthenticationTicket data) => Protect(data, null);

    /// <summary>
    /// Protect the authentication ticket and convert it to an encrypted string before sending
    /// out to the users. Additionally, specify the purpose of encryption, default is null.
    /// </summary>
    /// <param name=""data"">an instance of <see cref=""AuthenticationTicket""/></param>
    /// <param name=""purpose"">a purpose string</param>
    /// <returns>encrypted string representing the <see cref=""AuthenticationTicket""/></returns>
    public string Protect(AuthenticationTicket data, string purpose)
    {
        var array = _ticketSerializer.Serialize(data);

        return Base64UrlTextEncoder.Encode(_dataProtector.Protect(array));
    }
}
```
";
            var result = Markdown.ToHtml(code, _pipeline);
            Assert.NotEqual(result, code);
            Assert.False(string.IsNullOrWhiteSpace(result));
            AssertFencedCode(result, "csharp");
        }
    }
}
