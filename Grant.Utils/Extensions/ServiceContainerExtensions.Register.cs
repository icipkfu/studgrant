namespace Grant.Utils.Extensions
{
    using LightInject;

    public static partial class ServiceContainerExtensions
    {
        /// <summary>
        /// Зарегистрировать имплементацию как transient
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImpl"></typeparam>
        /// <param name="container"></param>
        public static void RegisterTransient<TService, TImpl>(this IServiceContainer container) where TImpl : TService
        {
            container.Register<TService, TImpl>();
        }

        /// <summary>
        /// Зарегистрировать имплементацию как transient
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImpl"></typeparam>
        /// <param name="container"></param>
        /// <param name="name"></param>
        public static void RegisterTransient<TService, TImpl>(this IServiceContainer container, string name) where TImpl : TService
        {
            container.Register<TService, TImpl>(name);
        }

		/// <summary>
		/// Зарегистрировать имплементацию как Singleton
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <typeparam name="TImpl"></typeparam>
		/// <param name="container"></param>
        public static void RegisterSingleton<TService, TImpl>(this IServiceContainer container) where TImpl : TService
        {
            container.Register<TService, TImpl>(new PerContainerLifetime());
        }

		/// <summary>
		/// Зарегистрировать имплементация как SessionScoped
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <typeparam name="TImpl"></typeparam>
		/// <param name="container"></param>
        public static void RegisterSessionScoped<TService, TImpl>(this IServiceContainer container) where TImpl : TService
        {
            container.Register<TService, TImpl>(new PerScopeLifetime());
        }

        /// <summary>
        /// Зарегистрировать имплементацию как PerRequest
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImpl"></typeparam>
        /// <param name="container"></param>
        public static void RegisterPerRequest<TService, TImpl>(this IServiceContainer container) where TImpl : TService
        {
            container.Register<TService, TImpl>(new PerRequestLifeTime());
        }
    }
}