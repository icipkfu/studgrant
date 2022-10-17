namespace Grant.Utils.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static partial class EnumerableExtensions
    {
        public static double SafeAvg(this IEnumerable<int> source)
        {
            ArgumentChecker.NotNull(source, "source");

            return source.Select(x => (int?)x).Average() ?? default(int);
        }

        public static double SafeAvg(this IEnumerable<long> source)
        {
            ArgumentChecker.NotNull(source, "source");

            return source.Select(x => (long?)x).Average() ?? default(long);
        }

        public static double SafeAvg(this IEnumerable<float> source)
        {
            ArgumentChecker.NotNull(source, "source");

            return source.Select(x => (float?)x).Average() ?? default(float);
        }

        public static double SafeAvg(this IEnumerable<double> source)
        {
            ArgumentChecker.NotNull(source, "source");

            return source.Select(x => (double?)x).Average() ?? default(double);
        }

        public static decimal SafeAvg(this IEnumerable<decimal> source)
        {
            ArgumentChecker.NotNull(source, "source");

            return source.Select(x => (decimal?)x).Average() ?? default(decimal);
        }

        public static double SafeAvg<T>(this IEnumerable<T> source, Func<T, int> selector)
        {
            ArgumentChecker.NotNull(source, "source");
            ArgumentChecker.NotNull(selector, "selector");

            return source.Select(selector).SafeAvg();
        }

        public static double SafeAvg<T>(this IEnumerable<T> source, Func<T, long> selector)
        {
            ArgumentChecker.NotNull(source, "source");
            ArgumentChecker.NotNull(selector, "selector");

            return source.Select(selector).SafeAvg();
        }

        public static double SafeAvg<T>(this IEnumerable<T> source, Func<T, float> selector)
        {
            ArgumentChecker.NotNull(source, "source");
            ArgumentChecker.NotNull(selector, "selector");

            return source.Select(selector).SafeAvg();
        }

        public static double SafeAvg<T>(this IEnumerable<T> source, Func<T, double> selector)
        {
            ArgumentChecker.NotNull(source, "source");
            ArgumentChecker.NotNull(selector, "selector");

            return source.Select(selector).SafeAvg();
        }

        public static decimal SafeAvg<T>(this IEnumerable<T> source, Func<T, decimal> selector)
        {
            ArgumentChecker.NotNull(source, "source");
            ArgumentChecker.NotNull(selector, "selector");

            return source.Select(selector).SafeAvg();
        }
    }
}