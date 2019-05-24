using System.Collections.Generic;
using System.Linq;

namespace UrlShortener.Services.Memory
{
    public class MemoryCacheService : ICacheService
    {
        private Dictionary<string, string> _store = new Dictionary<string, string>();
        public void SetCache(string key, string value)
        {
            if (!_store.Keys.Contains(key))
            {
                _store[key] = value;
            }
        }

        public string GetCache(string key)
        {
            return _store.Keys.Contains(key) ? _store[key] : string.Empty;

        }
    }
}
