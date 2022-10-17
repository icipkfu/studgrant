namespace Grant.Core.Config
{
    using System.Configuration;

    /// <summary>
    /// Элемент конфигурации модуля (соответствует секции Module).
    /// </summary>
    public class ModuleConfig : ConfigurationElement
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        [ConfigurationProperty("Id", IsKey = true, IsRequired = true)]
        public string Id
        {
            get
            {
                return (string)this["Id"];
            }

            set
            {
                this["Id"] = value;
            }
        }

        /// <summary>
        /// Параметры.
        /// </summary>
        [ConfigurationProperty("parameters")]
        public ModuleConfigParams Parameters
        {
            get
            {
                return (ModuleConfigParams)this["parameters"];
            }

            set
            {
                this["parameters"] = value;
            }
        }
    }
}
