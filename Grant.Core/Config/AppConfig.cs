using Grant.Utils.Extensions;

namespace Grant.Core.Config
{
    using System.Collections.Generic;
    using Utils;

    /// <summary>
    /// Конфигурация приложения
    /// </summary>
    public class AppConfig
    {
        private readonly Dictionary<string, Dictionary<string, object>> _storage;

        public Dictionary<string, object> this[string key] 
        {
            get { return _storage.Get(key) ?? new Dictionary<string, object>(); }
        }

        public AppConfig()
        {
            _storage = new Dictionary<string, Dictionary<string, object>>();
        }

        public void Add(string key, Dictionary<string, object> dict)
        {
            _storage[key] = dict;
        }

        public T GetAs<T>(string id, string key)
        {
            return _storage.Get(id).Return(x => x.Get(key)).To<T>();
        }

        public AppModuleConfig GetModuleConfig(string moduleId)
        {
            if (!_storage.ContainsKey(moduleId))
            {
                return new AppModuleConfig();
            }

            return new AppModuleConfig(_storage[moduleId]);
        }
    }
}