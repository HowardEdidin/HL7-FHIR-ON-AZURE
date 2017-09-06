#region Information

// Solution:  Spark
// Spark.Engine
// File:  StringExtensions.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:01 PM

#endregion

namespace FhirOnAzure.Search.Support
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public static class StringExtensions
    {
        public static string[] SplitNotInQuotes(this string value, char separator)
        {
            var parts = Regex.Split(value, separator + "(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)")
                .Select(s => s.Trim());

            return parts.ToArray();
        }

        public static string[] SplitNotEscaped(this string value, char separator)
        {
            var word = string.Empty;
            var result = new List<string>();
            var seenEscape = false;

            for (var i = 0; i < value.Length; i++)
            {
                if (value[i] == '\\')
                {
                    seenEscape = true;
                    continue;
                }

                if (value[i] == separator && !seenEscape)
                {
                    result.Add(word);
                    word = string.Empty;
                    continue;
                }

                if (seenEscape)
                {
                    word += '\\';
                    seenEscape = false;
                }

                word += value[i];
            }

            result.Add(word);

            return result.ToArray<string>();
        }

        public static Tuple<string, string> SplitLeft(this string text, char separator)
        {
            var pos = text.IndexOf(separator);

            if (pos == -1)
            {
                return Tuple.Create(text, (string) null); // Nothing to split
            }
            var key = text.Substring(0, pos);
            var value = text.Substring(pos + 1);

            return Tuple.Create(key, value);
        }
    }
}