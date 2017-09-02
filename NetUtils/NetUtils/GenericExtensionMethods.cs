using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Net3Migrations.Delegates;

namespace NetUtils
{
    public static class GenericExtensionMethods
    {
        public static bool IsGreaterThan<T>(this T value, T other) where T : IComparable
        { return value.CompareTo(other) > 0; }

        public static bool IsLessThan<T>(this T value, T other) where T : IComparable
        { return value.CompareTo(other) < 0; }

        public static T Max<T>(this T value2Compare, T value2CompareWith) where T : IComparable
        { return value2Compare.IsLessThan(value2CompareWith) ? value2CompareWith : value2Compare; }

        public static T DeepClone<T>(this T a)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, a);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }

        public static ICollection<T> AddRange<T>(this ICollection<T> collection2Add2, ICollection<T> collection2Add)
        {
            foreach (var item2Add in collection2Add)
            { collection2Add2.Add(item2Add); }
            return collection2Add2;
        }
        public static void RemoveAll<T>(this ICollection<T> collection2Clear, Trunc<T, bool> predicate)
        {
            for (var i = collection2Clear.Count - 1; i >= 0; i--)
            {
                var elementToRemove = collection2Clear.ElementAt(i);
                if (predicate(elementToRemove))
                    collection2Clear.Remove(elementToRemove);
            }
        }

        public static bool IsAnyOf<T>(this T item2Search, params T[] list2SearchIn) //where T : IComparable
        {
            return list2SearchIn.Contains(item2Search);
        }

        public static void AddRangeNullable<T>(this ICollection<T> list2Add2, ICollection<T> items2Add)
        {
            if (list2Add2 == null || items2Add == null) return;
            list2Add2.AddRange(items2Add);
        }
        public static void AddIfNotExistNullable<T>(this ICollection<T> toAddTo, T itemToAdd, IEqualityComparer<T> eq = null)
        {
            if (itemToAdd == null) return;
            if (eq != null)
            {
                if (!toAddTo.Contains(itemToAdd, eq))
                    toAddTo.Add(itemToAdd);
            }
            else
            {
                if (!toAddTo.Contains(itemToAdd))
                    toAddTo.Add(itemToAdd);
            }
        }
        public static void AddRangeDistinctNullable<T>(this ICollection<T> toAddTo, ICollection<T> secoond, IEqualityComparer<T> eq = null)
        {
            if (secoond == null) return;
            toAddTo.AddRange(secoond);
            ICollection<T> toAddToTemp = eq != null ? toAddTo.Distinct(eq).ToList() : toAddTo.Distinct().ToList();
            toAddTo.Clear();
            toAddTo.AddRange(toAddToTemp);
        }
        public static ICollection<T> AddRangeDistinctNullables<T>(ICollection<T> first, ICollection<T> secoond)
        {
            if (first == null && secoond == null) return null;
            ICollection<T> toAddTo = new List<T>();
            toAddTo.AddRange(first ?? new List<T>());
            toAddTo.AddRange(secoond ?? new List<T>());
            return toAddTo.Distinct().ToList();
        }
        public static bool AnyFrom<TSource>(
            this IEnumerable<TSource> source,
            Trunc<IEnumerable<TSource>, bool> predicate)
        {
            return predicate(source);
        }
        public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> source) => source == null || !source.Any();

        public static bool Between<T>(this T source, T from, T until, bool includeSelf = true) where T : IComparable
            => includeSelf ? source.CompareTo(@from) >= 0 && source.CompareTo(until) <= 0 ||
                             source.CompareTo(until) >= 0 && source.CompareTo(@from) <= 0
                : source.CompareTo(@from) > 0 && source.CompareTo(until) < 0 ||
                  source.CompareTo(until) > 0 && source.CompareTo(@from) < 0;

        public static bool BetweenStrict<T>(this T source, T from, T until) where T : IComparable => source.Between(@from, until, false);
    }
}