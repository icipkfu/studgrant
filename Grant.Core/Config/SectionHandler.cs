namespace Grant.Core.Config
{
    using System.Configuration;

    /// <summary>
    /// The B4 section handler.
    /// </summary>
    public class SectionHandler : ConfigurationSection
    {
        /// <summary>
        ///   The section name.
        /// </summary>
        public static readonly string SectionName = "appConfig";
        
        /// <summary>
        ///   Gets or sets Modules.
        /// </summary>
        [ConfigurationProperty("sections")]
        public B4AppSectionModules Sections
        {
            get
            {
                return (B4AppSectionModules)this["sections"];
            }

            set
            {
                this["sections"] = value;
            }
        }
    }

    /// <summary>
    /// The B4 app section modules.
    /// </summary>
    public class B4AppSectionModules : ConfigurationElement
    {
        
    }
}