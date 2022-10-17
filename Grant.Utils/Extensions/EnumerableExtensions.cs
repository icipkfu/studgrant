namespace Grant.Utils.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Методы расширения для объектов типа IEnumerable
    /// </summary>
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// The for each
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        public static void Foreach<T>(this IEnumerable<T> source, Action<T> action)
        {
            ArgumentChecker.NotNull(source, "source");
            ArgumentChecker.NotNull(action, "action");

            foreach (var item in source)
            {
                action(item);
            }
        }

        public static IEnumerable<T> Distinct<T, TMember>(this IEnumerable<T> source, Func<T, TMember> selector)
        {
            ArgumentChecker.NotNull(source, "source");
            ArgumentChecker.NotNull(selector, "selector");

            return source.GroupBy(selector).Select(x => x.First());
        }

        /// <summary>
        /// Проверка колекции на пустоту
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        /// <summary>
        /// Преобразование коллекции в HashSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            ArgumentChecker.NotNull(source, "source");

            return new HashSet<T>(source);
        }
    }
}