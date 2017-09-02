using System.Collections.Generic;
using Net3Migrations.Delegates;

namespace NetUtils
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<TSource> FromHierarchy<TSource>(
            this TSource source,
            Trunc<TSource, TSource> nextItem,
            Trunc<TSource, bool> canContinue)
        {
            for (var current = source; canContinue(current); current = nextItem(current))
            {
                yield return current;
            }
        }

        public static IEnumerable<TSource> FromHierarchy<TSource>(
            this TSource source,
            Trunc<TSource, TSource> nextItem)
            where TSource : class
        {
            return FromHierarchy(source, nextItem, s => s != null);
        }
        public static void ApplyToEach<T>(this IEnumerable<T> source, Fiction<T> func)
        {
            foreach (var elem in source)
            {
                func?.Invoke(elem);
            }
        }
    }
}