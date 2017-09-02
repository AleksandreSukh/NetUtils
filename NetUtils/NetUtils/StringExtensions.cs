using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetUtils
{
    public static class StringExtensions
    {
        public static string RemoveHtmlTags(this string toReplaceIn)
        {
            return Regex.Replace(toReplaceIn, "<[^>]*>", "");
        }
       
        public static void Clear(this StringBuilder value)
        {
            value.Length = 0;
            //value.Capacity = 0;
        }
        // Returns the string between str1 and str2
        public static string GetBetween(this string str, string str1, string str2)
        {
            int i1 = 0, i2 = 0;
            var rtn = "";

            i1 = str.IndexOf(str1, StringComparison.InvariantCultureIgnoreCase);
            if (i1 > -1)
            {
                i2 = str.IndexOf(str2, i1 + 1, StringComparison.InvariantCultureIgnoreCase);
                if (i2 > -1)
                {
                    rtn = str.Substring(i1 + str1.Length, i2 - i1 - str1.Length);
                }
            }
            return rtn;
        }

        public static MemoryStream ToStream(this string value, Encoding encoding)
        {
            return new MemoryStream(encoding.GetBytes(value ?? ""));
        }
        /// <summary>
        /// Merges continuous similar strings "I have some some good things".CompressMultiple("some ") == "I have some good things"
        /// </summary>
        /// <param name="toCompress"></param>
        /// <param name="toMerge"></param>
        /// <returns></returns>
        public static string CompressMultiple(this string toCompress, string toMerge)
        {
            if (!string.IsNullOrEmpty(toCompress) && !string.IsNullOrEmpty(toMerge) && toCompress.Contains(toMerge + toMerge))
                return toCompress.Replace(toMerge + toMerge, toMerge).CompressMultiple(toMerge);
            return toCompress;
        }

        public static string RemoveAfterLastOccurenceOf(this string toRemoveFrom, string last)
        {
            if (!string.IsNullOrEmpty(toRemoveFrom) && !string.IsNullOrEmpty(last) && toRemoveFrom.Contains(last))
            {
                var lastIndex = toRemoveFrom.LastIndexOf(last, StringComparison.Ordinal);
                var endIndex = lastIndex + last.Length;
                return toRemoveFrom.Remove(endIndex, toRemoveFrom.Length - endIndex);
            }
            return toRemoveFrom;
        }
        public static string RemoveAfterLastInc(this string toRemoveFrom, string last)
        {
            if (!string.IsNullOrEmpty(toRemoveFrom) && !string.IsNullOrEmpty(last) && toRemoveFrom.Contains(last))
            {
                var lastIndex = toRemoveFrom.LastIndexOf(last, StringComparison.Ordinal);
                return toRemoveFrom.Remove(lastIndex, toRemoveFrom.Length - lastIndex);
            }
            return toRemoveFrom;
        }
        public static string RemoveBeforeLastInc(this string toRemoveFrom, string last)
        {
            if (!string.IsNullOrEmpty(toRemoveFrom) && !string.IsNullOrEmpty(last) && toRemoveFrom.Contains(last))
                return toRemoveFrom.Substring(toRemoveFrom.LastIndexOf(last, StringComparison.Ordinal) + last.Length);
            return toRemoveFrom;
        }
        public static bool ContainsStringInBrackets(this string toRemoveFrom, string firstBracket, string lastBracket)
        {
            if (
                !string.IsNullOrEmpty(toRemoveFrom)
                && !string.IsNullOrEmpty(firstBracket)
                && toRemoveFrom.Contains(firstBracket)
                && !string.IsNullOrEmpty(lastBracket)
                && toRemoveFrom.Contains(lastBracket)
            )
            {
                var indexOfFirst = toRemoveFrom.IndexOf(firstBracket, StringComparison.Ordinal);
                int indexOfLast = toRemoveFrom.IndexOfAfter(lastBracket, indexOfFirst);
                return indexOfFirst < indexOfLast;
            }
            return false;
        }
        public static string RemoveFirstInBrackets(this string toRemoveFrom, string firstBracket, string lastBracket)
        {
            if (
                !string.IsNullOrEmpty(toRemoveFrom)
                && !string.IsNullOrEmpty(firstBracket)
                && toRemoveFrom.Contains(firstBracket)
                && !string.IsNullOrEmpty(lastBracket)
                && toRemoveFrom.Contains(lastBracket)
            )
            {
                var indexOfFirst = toRemoveFrom.IndexOf(firstBracket, StringComparison.Ordinal);
                int indexOfLast = toRemoveFrom.IndexOfAfter(lastBracket, indexOfFirst);
                return toRemoveFrom.Remove(indexOfFirst, indexOfLast + lastBracket.Length - indexOfFirst);
            }
            return toRemoveFrom;
        }
        public static string GetFirstWithBrackets(this string toGetFrom, string firstBracket, string lastBracket)
        {
            if (
                !string.IsNullOrEmpty(toGetFrom)
                && !string.IsNullOrEmpty(firstBracket)
                && toGetFrom.Contains(firstBracket)
                && !string.IsNullOrEmpty(lastBracket)
                && toGetFrom.Contains(lastBracket)
            )
            {
                var indexOfFirst = toGetFrom.IndexOf(firstBracket, StringComparison.Ordinal);
                int indexOfLast = toGetFrom.IndexOfAfter(lastBracket, indexOfFirst);
                return toGetFrom.Substring(indexOfFirst, indexOfLast + lastBracket.Length - indexOfFirst);
            }
            return toGetFrom;
        }
        public static string GetFirstInBrackets(this string toGetFrom, string firstBracket, string lastBracket)
        {
            var withBrackets = toGetFrom.GetFirstWithBrackets(firstBracket, lastBracket);
            return withBrackets.Substring(firstBracket.Length, withBrackets.Length - (firstBracket.Length + lastBracket.Length));
        }
        public static string RemoveAllWithBrackets(this string toRemoveFrom, string firstBracket, string lastBracket)
        {
            while (toRemoveFrom.ContainsStringInBrackets(firstBracket, lastBracket))
            {
                toRemoveFrom = toRemoveFrom.RemoveFirstInBrackets(firstBracket, lastBracket);
            }
            return toRemoveFrom;
        }
        //TODO:Refactor Take to frameWorks

        public static IEnumerable<string> SplitByString(this string thisString, string splitBy, bool ignoreCase = false)
        {
            if (thisString == null || splitBy == null) throw new ArgumentNullException();
            if (splitBy.Length == 0) throw new InvalidOperationException("String cannot be split by empty string");
            var result = new List<string>();
            var stringArg = thisString;
            while (ignoreCase ? stringArg.ContainsIgnoreCaseInvariant(splitBy) : stringArg.Contains(splitBy))
            {
                var chunkLength = stringArg.IndexOf(splitBy, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
                var chunk = stringArg.Substring(0, chunkLength);
                result.Add(chunk);
                stringArg = stringArg.Remove(0, chunkLength + splitBy.Length);
            }
            result.Add(stringArg);
            return result;
        }
        public static string ReplaceFirstWithBrackets(this string toReplaceIn, string newString, string firstBracket, string lastBracket)
        {
            if (
                !string.IsNullOrEmpty(toReplaceIn)
                && !string.IsNullOrEmpty(firstBracket)
                && toReplaceIn.Contains(firstBracket)
                && !string.IsNullOrEmpty(lastBracket)
                && toReplaceIn.Contains(lastBracket)
            )
            {
                var indexOfFirst = toReplaceIn.IndexOf(firstBracket, StringComparison.Ordinal);
                int indexOfLast = toReplaceIn.IndexOfAfter(lastBracket, indexOfFirst);
                return toReplaceIn.Remove(indexOfFirst, indexOfLast + lastBracket.Length - indexOfFirst).Insert(indexOfFirst, newString);
            }
            return toReplaceIn;
        }
        public static int IndexOfAfter(this string toSearchIn, string findIndexOf, int afterIndex)
        {
            var indexInSubstring = toSearchIn.Substring(afterIndex + 1).IndexOf(findIndexOf, StringComparison.Ordinal);
            return indexInSubstring > -1 ? indexInSubstring + afterIndex + 1 : -1;
        }
        public static string ReplaceAllWithRegex(this string toReplaceIn, string newString, string pattern)
        {
            return Regex.Replace(toReplaceIn, pattern, newString);
        }
        public static string ReplaceAllWithBrackets(this string toReplaceIn, string newString, string firstBracket, string lastBracket)
        {
            while (toReplaceIn.ContainsStringInBrackets(firstBracket, lastBracket))
            {
                toReplaceIn = toReplaceIn.ReplaceFirstWithBrackets(newString, firstBracket, lastBracket);
            }
            return toReplaceIn;
        }
        public static int CountAllWithBrackets(this string toCountIn, string firstBracket, string lastBracket)
        {
            if (toCountIn.ContainsStringInBrackets(firstBracket, lastBracket))
            {
                return 1 + toCountIn.RemoveFirstInBrackets(firstBracket, lastBracket).CountAllWithBrackets(firstBracket, lastBracket);
            }
            return 0;
        }
        public static string RemoveBeforeLast(this string toRemoveFrom, string last)
        {
            if (!string.IsNullOrEmpty(toRemoveFrom) && !string.IsNullOrEmpty(last) && toRemoveFrom.Contains(last))
                return toRemoveFrom.Substring(toRemoveFrom.LastIndexOf(last, StringComparison.Ordinal));
            return toRemoveFrom;
        }

        public static string FormatWith(this string formatString, params object[] paramValues)
        { return string.Format(formatString, paramValues); }

        public static bool ContainsIgnoreCaseInvariant(this string string2Check, string containsString)
        {
            if (string2Check == null || containsString == null)
                return false;
            return string2Check.ToLowerInvariant().Contains(containsString.ToLowerInvariant());
        }
        public static string Append(this string source, string str2)
        {
            source += str2;
            return source;
        }
        public static string ToValidFileName(this string stringToFileName)
        {
            string result = stringToFileName;
            foreach (var nameChar in Path.GetInvalidFileNameChars())
                result = result.Replace(nameChar, '_');
            return result;
        }

        public static string ToValidFilePath(this string stringToFilePath)
        {
            return Path.GetInvalidPathChars().Aggregate(stringToFilePath, (current, c) => current.Replace(c, '_'));
        }
        public static bool EndsWithAnyOf(this string string2Check, params string[] characters)
        {
            return characters.Any(string2Check.EndsWith);
        }
        public static bool StartsWithAnyOf(this string string2Check, params string[] characters)
        {
            return characters.Any(string2Check.StartsWith);
        }
        public static bool ContainsAnyOf(this string string2Check, params string[] strings)
        {
            return strings.Any(string2Check.Contains);
        }
        public static bool ContainsAnyOf(this string string2Check, params char[] characters)
        {
            return characters.Any(string2Check.Contains);
        }
        public static bool ContainsAnyOf(this char char2Check, params string[] characters)
        {
            return characters.Any(char2Check.ToString().Contains);
        }
    }


}
