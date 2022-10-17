using Grant.Utils.Extensions;

namespace Grant.Core.Config
{
    using System.Configuration;
    using Utils;

    public class KeyValueSectionParameter : ConfigurationElement
    {
        /// <summary>
        /// Ключ.
        /// </summary>
        [ConfigurationProperty("key", IsKey = true, IsRequired = true)]
        public string Key
        {
            get { return this["key"].ToInvariantString(); }
            set { this["key"] = value; }
        }

        /// <summary>
        /// Значение.
        /// </summary>
        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get { return this["value"].ToInvariantString(); }
            set { this["value"] = value; }
        }
    }
}
