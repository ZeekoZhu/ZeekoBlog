using System.Reflection;

namespace ZeekoBlog.Markdown
{
    public abstract class BaseMarkdownPlugin
    {
        /// <summary>
        /// Plugin id
        /// </summary>
        /// <example>com.example.AwesomePlugin</example>
        public abstract string Id { get; }

        public abstract MarkdownOutput Invoke(MarkdownOutput output);

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
