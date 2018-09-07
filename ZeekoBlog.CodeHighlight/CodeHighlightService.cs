using System;
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

        public CodeHighlightService(INodeServices node)
        {
            _node = node;
        }
        public async Task<HighlightResult> HighlightAsync(string source, string lang = null)
        {
            try
            {
                var result = await _node.InvokeAsync<HighlightResult>("./scripts/highlight", source, lang);
                result.IsSuccess = true;
                return result;
            }
            catch
            {
                return new HighlightResult
                {
                    IsSuccess = false, Result = source, Language = lang
                };
            }
        }
    }

    public static class CodeHighlightServiceExt
    {
        public static IServiceCollection AddCodeHighlight(this IServiceCollection services, Action<NodeServicesOptions> setupOptions)
        {
            services.AddNodeServices(setupOptions);
            services.AddSingleton<CodeHighlightService>();
            return services;
        }

        public static IServiceCollection AddCodeHighlight(this IServiceCollection services)
        {
            services.AddNodeServices();
            services.AddSingleton<CodeHighlightService>();
            return services;
        }
    }
}
