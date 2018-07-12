using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace ZeekoBlog.Markdown
{
    public class MarkdownService
    {

        private readonly List<BaseMarkdownPlugin> _plugins = new List<BaseMarkdownPlugin>();
        public MarkdownService Add(BaseMarkdownPlugin plugin)
        {
            _plugins.Add(plugin);
            return this;
        }

        public MarkdownOutput Process(string md)
        {
            var output = new MarkdownOutput
            {
                Source = md,
                Storage = new PluginStorage()
            };
            foreach (var plugin in _plugins)
            {
                output = plugin.Invoke(output);
            }

            return output;
        }
    }

    public class MarkdownServiceBuilder
    {
        internal List<Type> PluginTypes = new List<Type>();
        internal List<BaseMarkdownPlugin> Plugins = new List<BaseMarkdownPlugin>();
        public MarkdownServiceBuilder Add<T>() where T : BaseMarkdownPlugin
        {
            PluginTypes.Add(typeof(T));
            return this;
        }

        public MarkdownServiceBuilder Add(BaseMarkdownPlugin plugin)
        {
            Plugins.Add(plugin);
            return this;
        }
    }
    public static class MarkdownServiceExtensions
    {
        public static void AddMarkdownService(this IServiceCollection services, Action<MarkdownServiceBuilder> config)
        {
            var builder = new MarkdownServiceBuilder();
            config(builder);
            foreach (var pluginType in builder.PluginTypes)
            {
                if (services.Any(s => s.ServiceType == pluginType) == false)
                {
                    services.AddScoped(pluginType);
                }
            }
            services.AddScoped<MarkdownService>(provider =>
            {
                var mdSvc = new MarkdownService();
                foreach (var pluginType in builder.PluginTypes)
                {
                    var plugin = provider.GetService(pluginType) as BaseMarkdownPlugin;
                    if (plugin == null)
                    {
                        throw new InvalidOperationException("Can not find plugin provider");
                    }

                    mdSvc.Add(plugin);
                }

                foreach (var plugin in builder.Plugins)
                {
                    if (plugin == null)
                    {
                        throw new ArgumentException("Plugin can not be null");
                    }
                }

                return mdSvc;
            });
        }
    }
}
