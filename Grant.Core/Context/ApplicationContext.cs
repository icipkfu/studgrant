namespace Grant.Core.Context
{
    using LightInject;
    using Utils.Extensions;

    /// <summary>
    /// Статический класс контекста приложения
    /// </summary>
    public static class ApplicationContext
    {
        public static void Initialize<TContext>() where TContext : IAppContext
        {
            var container = new ServiceContainer();

            container.Register<IServiceContainer>(factory => container);

            container.RegisterSingleton<IAppContext, TContext>();

            Current = container.GetInstance<IAppContext>();

            Current.Start();
        }

        /// <summary>
        /// Текущий контекст приложения
        /// </summary>
        public static IAppContext Current { get; private set; }
    }
}