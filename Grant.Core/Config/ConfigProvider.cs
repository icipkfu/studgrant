using Grant.Utils.Extensions;

namespace Grant.Core.Config
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using Config;
    using Context;
 
    using Utils;

    /// <summary>
    /// Имплементация поставщика настроек
    /// </summary>
    public class ConfigProvider : IConfigProvider
    {
        /// <summary>
        /// собстна, настройки
        /// </summary>
        private AppConfig _config;

        /// <summary>
        /// Получить текущую конфигурацию приложения
        /// </summary>
        /// <returns></returns>
        public AppConfig GetConfig()
        {
            if (_config == null)
            {
                RefreshConfig();
            }

            return _config;
        }

        /// <summary>
        /// Перечитать настройки
        /// </summary>
        public void RefreshConfig()
        {
            var fileMap = new ExeConfigurationFileMap {ExeConfigFilename = GetConfigFilePathForLoading()};

            var configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            _config = new AppConfig();

            var modulesSection = configuration.GetSection(ModulesSectionHandler.SectionName) as ModulesSectionHandler;

            if (modulesSection != null && modulesSection.Modules != null)
            {
                for (var i = 0; i < modulesSection.Modules.Count; i++)
                {
                    var module = modulesSection.Modules[i];

                    var moduleParams = new Dictionary<string, object>();
                    var dicParams = module.Parameters
                        .Cast<KeyValueSectionParameter>()
                        .ToDictionary(parameter => parameter.Key, parameter => parameter.Value);

                    moduleParams.Apply(dicParams);

                    _config.Add(module.Id, moduleParams);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public AppModuleConfig GetModuleConfig(string key)
        {
            return GetConfig().GetModuleConfig(key);
        }

        private string GetConfigFilePathForLoading()
        {
            var appConfigFileName = "web.config";

            if (File.Exists(GetConfigFilePath("Grant.Config")))
            {
                appConfigFileName = "Grant.Config";
            }

            return GetConfigFilePath(appConfigFileName);
        }

        private string GetConfigFilePath(string virtualFilePath)
        {
            var path = ApplicationContext.Current.MapPath(virtualFilePath);

            if (path.IsEmpty())
            {
                path = virtualFilePath.Replace("~/", "");
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            }

            return path;
        }

        /// <summary>
        /// Сохранить изменения
        /// </summary>
        public void Save()
        {
            
        }
    }
}