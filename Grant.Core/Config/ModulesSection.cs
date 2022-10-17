namespace Grant.Core.Config
{
    using System.Collections.Generic;
    using System.Configuration;

    [ConfigurationCollection(typeof (ModuleConfig), AddItemName = "section", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class ModulesSection : ConfigurationElementCollection
    {
        /// <summary>
        /// Обновляет элементы конфигурации модулей.
        /// </summary>
        /// <param name="modules">
        /// </param>
        public void UpdateFrom(Dictionary<string, Dictionary<string, string>> modules)
        {
            Clear();
            foreach (var key in modules.Keys)
            {
                var configParams = new ModuleConfigParams();
                configParams.UpdateFrom(modules[key]);

                this.BaseAdd(new ModuleConfig {Id = key, Parameters = configParams});
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ModuleConfig();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ModuleConfig) element).Id;
        }

        /// <summary>
        /// Добавляет элемент конфигурации.
        /// </summary>
        /// <param name="element"></param>
        public void Add(ModuleConfig element)
        {
            BaseAdd(element);
        }

        /// <summary>
        /// Удаляет все элементы конфигурации.
        /// </summary>
        public void Clear()
        {
            BaseClear();
        }

        /// <summary>
        /// Возвращает индекс элемента конфигурации в коллекции.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public int IndexOf(ModuleConfig element)
        {
            return BaseIndexOf(element);
        }

        /// <summary>
        /// Удаляет элемент конфигурации.
        /// </summary>
        public void Remove(ModuleConfig element)
        {
            if (BaseIndexOf(element) >= 0)
            {
                BaseRemove(element.Id);
            }
        }

        /// <summary>
        /// Удаляет элемент конфигурации по индексу.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        /// <summary>
        /// Индексатор коллекции.
        /// </summary>
        /// <param name="index"></param>
        public ModuleConfig this[int index]
        {
            get { return (ModuleConfig) BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }
    }
}