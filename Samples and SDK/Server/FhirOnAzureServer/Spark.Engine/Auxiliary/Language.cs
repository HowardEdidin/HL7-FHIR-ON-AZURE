#region Information

// Solution:  Spark
// Spark.Engine
// File:  Language.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:50 PM

#endregion

namespace FhirOnAzure.Engine.Auxiliary
{
    using System;

    /// <summary>
    ///     Helpes converting variables to human readable text
    /// </summary>
    public static class Language
    {
        public static string Since(DateTimeOffset? since)
        {
            return since != null ? since.ToString() : "the dawn of man";
        }

        public static string Number<T>(T item, int count)
        {
            var name = item.GetType().ToString();
            switch (count)
            {
                case 0: return string.Format("no {0}s", name);
                case 1: return string.Format("one {0}", name);
                default: return string.Format("{0} {1}s", count, name);
            }
        }
    }
}