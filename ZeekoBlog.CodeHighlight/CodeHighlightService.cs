using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.DependencyInjection;

namespace ZeekoBlog.CodeHighlight
{
    public class HighlightResult
    {
        public bool IsSuccess { get; set; }
        public string Result { get; set; }
        public string Language { get; set; }
    }

    public class CodeHighlightService
    {
        private readonly INodeServices _node;
        private readonly string[] _bypass;

        public CodeHighlightService(INodeServices node, string[] bypass)
        {
            _node = node;
            _bypass = bypass ?? new string[] { };
            _bypass = _bypass.Select(x => x.ToLowerInvariant()).ToArray();
        }

        public async Task<HighlightResult> HighlightAsync(string source, string lang = null)
        {
            if (_bypass.Contains(lang))
            {
                return new HighlightResult
                {
                    IsSuccess = false,
                    Result = source,
                    Language = lang
                };
            }

            try
            {
                var scriptPath = Path.Combine(
                    Path.GetDirectoryName(typeof(CodeHighlightService).Assembly.Location) ??
                    throw new InvalidOperationException(),
                    "codehighlight-scripts",
                    "code-highlight");
                var result = await _node.InvokeAsync<HighlightResult>(scriptPath, source, lang);
                result.IsSuccess = true;
                result.Result = result.Result.Trim();
                return result;
            }
            catch
            {
                return new HighlightResult
                {
                    IsSuccess = false,
                    Result = source,
                    Language = lang
                };
            }
        }
    }

    public static class CodeHighlightServiceExt
    {
        public static IServiceCollection AddCodeHighlight(this IServiceCollection services, string[] bypass = null)
        {
            services.AddSingleton(
                provider =>
                {
                    var node = provider.GetService<INodeServices>();
                    return new CodeHighlightService(node, bypass);
                });
            return services;
        }
    }
}