namespace Grant.Utils.Extensions
{
    using System.Collections.Generic;

    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Получить первый непустой объект
        /// </summary>
        public static T Coalesce<T>(this IEnumerable<T> source, T defaultValue = default (T)) where T : class
        {
            ArgumentChecker.NotNull(source, "source");

            foreach (var item in source)
            {
                if (item != null)
                    return item;
            }

            return defaultValue;
        }

        /// <summary>
        /// Получить первый непустой объект
        /// </summary>
        public static T Coalesce<T>(this IEnumerable<T?> source, T defaultValue = default (T)) where T : struct
        {
            ArgumentChecker.NotNull(source, "source");

            foreach (var item in source)
            {
                if (item.HasValue)
                    return item.Value;
            }

            return defaultValue;
        }

        /// <summary>
        /// Получить первую непустую строку
        /// </summary>
        public static string Coalesce(this IEnumerable<string> source, string defaultValue = null)
        {
            ArgumentChecker.NotNull(source, "source");

            foreach (var item in source)
            {
                if (!item.IsEmpty())
                    return item;
            }

            return defaultValue;
        }
    }
}