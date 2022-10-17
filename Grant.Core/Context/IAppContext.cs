namespace Grant.Core.Context
{
    using LightInject;
    using Logging;

    /// <summary>
    /// Интерфейс контекста приложения
    /// </summary>
    public interface IAppContext
    {
        /// <summary>
        /// Контейнер
        /// </summary>
        IServiceContainer Container { get; }

        /// <summary>
        /// Логгер
        /// </summary>
      //  ILogManager Log { get; }

        /// <summary>
        /// Старт
        /// </summary>
        void Start();

        /// <summary>
        /// Остановка
        /// </summary>
        void Stop();

        /// <summary>
        /// Сопоставляет виртуальный путь с физическим путем на сервере.
        /// </summary>
        /// <param name="virtualPath">
        /// Виртуальный путь.
        /// </param>
        /// <returns></returns>
        string MapPath(string virtualPath);

        string ApplicationPhysicalPath();

        string CurUserId();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string GetUrl();
    }
}