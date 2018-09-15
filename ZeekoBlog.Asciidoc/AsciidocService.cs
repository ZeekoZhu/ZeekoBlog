using Microsoft.AspNetCore.NodeServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ZeekoBlog.Core.Models;

namespace ZeekoBlog.AsciiDoc
{
    public class AsciiDocRenderedResult
    {
        public string Source { get; set; }
        public string Value { get; set; }
        public string[] Languages { get; set; }
        public TOCItem[] TableOfContents { get; set; }
    }
    public class AsciiDocService
    {
        private readonly string[] _bypass;
        private readonly INodeServices _node;
        public AsciiDocService(INodeServices node, string[] bypass)
        {
            _node = node;
            _bypass = bypass ?? new string[] { };
            _bypass = _bypass.Select(x => x.ToLowerInvariant()).ToArray();
        }

        public async Task<AsciiDocRenderedResult> Process(string source)
        {
            try
            {
                var scriptPath = Path.Combine(
                            Path.GetDirectoryName(typeof(AsciiDocService).Assembly.Location) ?? throw new InvalidOperationException(),
                            "asciidoc-scripts", "asciidoc");
                var result = await _node.InvokeAsync<AsciiDocRenderedResult>(scriptPath, source, _bypass);
                result.TableOfContents = result.TableOfContents ?? new TOCItem[] { };
                for (int i = 0; i < result.TableOfContents.Length; i++)
                {
                    result.TableOfContents[i].Order = i;
                }
                return result;
            }
            catch
            {
                return new AsciiDocRenderedResult
                {
                    Languages = new string[] { },
                    Source = source,
                    Value = source,
                    TableOfContents = new TOCItem[] { }
                };
            }
        }

    }

    public static class AsciiDocServiceExt
    {
        public static IServiceCollection AddAsciiDoc(this IServiceCollection services, string[] bypass = null)
        {
            services.AddSingleton(provider =>
            {
                var node = provider.GetService<INodeServices>();
                return new AsciiDocService(node, bypass);
            });
            return services;
        }
    }
}
