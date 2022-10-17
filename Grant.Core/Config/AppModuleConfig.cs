using Grant.Utils.Extensions;

namespace Grant.Core.Config
{
    using System.Collections.Generic;
    using Utils;

    public class AppModuleConfig
    {
        private readonly Dictionary<string, object> _values;

        internal AppModuleConfig()
        {
            _values = new Dictionary<string, object>();
        }

        internal AppModuleConfig(Dictionary<string, object> values)
        {
            _values = values;
        }

        public T GetAs<T>(string key)
        {
            return _values.Get(key).To<T>();
        }
    }
}