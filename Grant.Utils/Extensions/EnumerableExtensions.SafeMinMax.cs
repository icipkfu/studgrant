namespace Grant.Utils.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static partial class EnumerableExtensions
    {
        public static T SafeMin<T>(this IEnumerable<T> source, T defaultValue = default (T)) where T : struct
        {
            ArgumentChecker.NotNull(source, "source");

            return source.Select(x => (T?)x).Min() ?? defaultValue;
        }

        public static T SafeMax<T>(this IEnumerable<T> source, T defaultValue = default (T)) where T : struct
        {
            ArgumentChecker.NotNull(source, "source");

            return source.Select(x => (T?)x).Max() ?? defaultValue;
        }

        public static TOut SafeMin<TIn, TOut>(
            this IEnumerable<TIn> source,
            Func<TIn, TOut> selector,
            TOut defaultValue = default (TOut))
            where TOut : struct
        {
            ArgumentChecker.NotNull(source, "source");
            ArgumentChecker.NotNull(selector, "selector");

            return source.Select(selector).Select(x => (TOut?)x).Min() ?? defaultValue;
        }

        public static TOut SafeMax<TIn, TOut>(
            this IEnumerable<TIn> source,
            Func<TIn, TOut> selector,
            TOut defaultValue = default (TOut))
            where TOut : struct
        {
            ArgumentChecker.NotNull(source, "source");
            ArgumentChecker.NotNull(selector, "selector");

            return source.Select(selector).Select(x => (TOut?)x).Max() ?? defaultValue;
        }
    }
}