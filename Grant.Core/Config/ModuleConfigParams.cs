namespace Grant.Core.Config
{
    using System.Collections.Generic;
    using System.Configuration;

    public class ModuleConfigParams : ConfigurationElementCollection
    {
        /// <summary>
        /// Обновление коллекции параметров модуля.
        /// </summary>
        /// <param name="appSettings">
        /// Словарь, содержащий параметры в виде ключ - значение.
        /// </param>
        public void UpdateFrom(Dictionary<string, string> appSettings)
        {
            this.BaseClear();
            foreach (var key in appSettings)
            {
                this.BaseAdd(new KeyValueSectionParameter { Key = key.Key, Value = key.Value });
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new KeyValueSectionParameter();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((KeyValueSectionParameter)element).Key;
        }
    }
}
