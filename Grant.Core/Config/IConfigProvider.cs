namespace Grant.Core.Config
{
    /// <summary>
    /// Поставщик Оо настроек
    /// </summary>
    public interface IConfigProvider
    {
        /// <summary>
        /// Получить текущую конфигурацию приложения
        /// </summary>
        /// <returns></returns>
        AppConfig GetConfig();

        /// <summary>
        /// Получить конфиг модуля
        /// </summary>
        /// <returns></returns>
        AppModuleConfig GetModuleConfig(string key);

        /// <summary>
        /// Перечитать настройки
        /// </summary>
        void RefreshConfig();

        /// <summary>
        /// Сохранить изменения
        /// </summary>
        void Save();
    }
}