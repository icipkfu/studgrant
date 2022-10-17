namespace Grant.Utils.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Объединение коллекции строк в одну через разделитель
        /// </summary>
        public static string Aggregate(this IEnumerable<string> source, string separator = ",")
        {
            ArgumentChecker.NotNull(source, "source");

            var sb = new StringBuilder();

            foreach (var item in source)
            {
                if (string.IsNullOrEmpty(item)) continue;

                if (sb.Length > 0)
                    sb.Append(separator);

                sb.Append(item);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Объединение коллекции объектов в строку через разделитель
        /// </summary>
        public static string Aggregate<T>(this IEnumerable<T> source, Func<T, string> selector, string separator = ",")
        {
            ArgumentChecker.NotNull(source, "source");
            ArgumentChecker.NotNull(selector, "selector");

            return source.Select(selector).Aggregate(separator);
        }

        /// <summary>
        /// Объединение коллекции объектов в строку через разделитель
        /// </summary>
        public static string Aggregate<T>(this IEnumerable<T> source, Func<T, object> selector, string separator = ",")
        {
            ArgumentChecker.NotNull(source, "source");
            ArgumentChecker.NotNull(selector, "selector");

            var sb = new StringBuilder();

            foreach (var item in source)
            {
                var value = selector(item);

                if (value == null) continue;

                if (sb.Length > 0)
                    sb.Append(separator);

                sb.Append(item);
            }

            return sb.ToString();
        }
    }
}