namespace Grant.Utils.Extensions
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    public static class QueryableExtensions
    {
        public static IQueryable<T> WhereIf<T>(
            this IQueryable<T> source,
            bool condition,
            Expression<Func<T, bool>> selector)
        {
            ArgumentChecker.NotNull(source, "source");
            ArgumentChecker.NotNull(selector, "selector");

            if (condition)
                source = source.Where(selector);

            return source;
        }

        public static IQueryable<T> Paging<T>(this IQueryable<T> source, int skip, int limit)
        {
            if (limit <= 0 || limit > 500)
            {
                limit = 25;
            }

            if (skip < 0)
            {
                skip = 0;
            }

            return source
                .Skip(skip)
                .Take(limit);
        }
    }
}