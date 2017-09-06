#region Information

// Solution:  Spark
// Spark.Engine
// File:  NullExtensions.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:01 PM

#endregion

namespace FhirOnAzure.Search.Support
{
    using System.Collections;
    using Hl7.Fhir.Model;

    public static class NullExtensions
    {
        public static bool IsNullOrEmpty(this IList list)
        {
            if (list == null) return true;

            return list.Count == 0;
        }

        public static bool IsNullOrEmpty(this Primitive element)
        {
            if (element == null) return true;

            if (element.ObjectValue == null) return true;

            return true;
        }
    }
}