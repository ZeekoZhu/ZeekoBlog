using System.Collections.Generic;

namespace ZeekoBlog.Markdown
{
    public class PluginStorage
    {
        private readonly Dictionary<string, object> _storage = new Dictionary<string, object>();

        public (bool IsSucess, TStorage Value) TryGet<TStorage>(string pluginId)
        {
            do
            {

                if (_storage.TryGetValue(pluginId, out var value) == false)
                {
                    break;
                }

                if (!(value is TStorage result))
                {
                    break;
                }

                return (true, result);
            } while (false);
            return (false, default(TStorage));
        }

        public void Upsert<TStorage>(string pluginId, TStorage storage)
        {
            if (_storage.ContainsKey(pluginId))
            {
                _storage[pluginId] = storage;
            }
            else
            {
                _storage.Add(pluginId, storage);
            }
        }
    }
}