namespace ZeekoBlog.Markdown
{
    public abstract class MarkdownPlugin
    {
        /// <summary>
        /// Plugin id
        /// </summary>
        /// <example>com.example.AwesomePlugin</example>
        public abstract string Id { get; }

        public abstract MarkdownOutput Invoke(MarkdownOutput output);

        public (bool IsSucess, TStorage Storage) TryGet<TStorage>(PluginStorage storage)
        {
            return storage.TryGet<TStorage>(Id);
        }
    }
}