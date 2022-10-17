namespace Grant.Utils.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using LightInject;

    public static partial class ServiceContainerExtensions
    {
        /// <summary>
        /// Получить экземпляр объекта из контейнера
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <returns></returns>
        public static T Get<T>(this IServiceContainer container)
        {
            return container.GetInstance<T>();
        }

        /// <summary>
        /// Получить экземпляр объекта из контейнера по имени
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T Get<T>(this IServiceContainer container, string name)
        {
            return container.GetInstance<T>(name);
        }

        public static T[] GetAll<T>(this IServiceContainer container)
        {
            return container.Get<IEnumerable<T>>().ToArray();
        }
    }
}