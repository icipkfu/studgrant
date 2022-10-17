namespace Grant.Core.Install
{
    /// <summary>
    /// Интерфейс обработчика, выполняющегося после всех инсталлеров
    /// </summary>
    public interface IAfterInstallInterceptor
    {
        /// <summary>
        /// Запустить обработчик
        /// </summary>
        void Run();
    }
}