using System;
using System.Reflection;
using System.Threading.Tasks;

namespace ZeekoBlog.Markdown
{
    public abstract class BaseMarkdownPlugin
    {
        /// <summary>
        /// Plugin id
        /// </summary>
        /// <example>com.example.AwesomePlugin</example>
        public abstract string Id { get; }

        public virtual Task<MarkdownOutput> InvokeAsync(MarkdownOutput output)
        {
            return Task.FromResult(Invoke(output));
        }

        public virtual MarkdownOutput Invoke(MarkdownOutput output)
        {
            throw new NotImplementedException("You must override either Invoke or InvokeAsync");
        }

    }

    public abstract class MarkdownPlugin<TData> : BaseMarkdownPlugin
    {
        public static (bool IsSucess, TData Value) TryGet<TPlugin>(PluginStorage storage) where TPlugin : MarkdownPlugin<TData>
        {
            var pluginType = typeof(TPlugin);
            var idInfo = pluginType.GetField("ID", BindingFlags.Static | BindingFlags.Public);
            var id = idInfo.GetValue(null) as string;
            return storage.TryGet<TData>(id);
        }
    }
}
