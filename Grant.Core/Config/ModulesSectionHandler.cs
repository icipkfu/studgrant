namespace Grant.Core.Config
{
    using System.Configuration;

    public class ModulesSectionHandler : ConfigurationSection
    {
        /// <summary>
        /// Имя секции.
        /// </summary>
        public static readonly string SectionName = "sections";

        private static readonly ConfigurationProperty _propModulesSection = new ConfigurationProperty(null, typeof(ModulesSection), null, ConfigurationPropertyOptions.IsDefaultCollection);

        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        static ModulesSectionHandler()
        {
            _properties.Add(_propModulesSection);
        }

        /// <summary>
        /// Возвращает коллекцию элементов конфигурации модулей.
        /// </summary>
        [ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public ModulesSection Modules
        {
            get
            {
                return (ModulesSection)base[_propModulesSection];
            }
        }
    }
}