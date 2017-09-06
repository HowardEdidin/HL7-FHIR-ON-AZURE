#region Information

// Solution:  Spark
// FhirOnAzure
// File:  Examples.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:42 PM

#endregion

namespace FhirOnAzure.Import
{
    using System;
    using System.Collections.Generic;
    using Embedded;
    using Engine.Extensions;
    using Hl7.Fhir.Model;

    public static class Examples
    {
        public static IEnumerable<Resource> ImportEmbeddedZip()
        {
            return Resources.ExamplesZip.ExtractResourcesFromZip();
        }

        public static IEnumerable<Resource> LimitPerType(this IEnumerable<Resource> resources, int amount)
        {
            var counters = new Dictionary<string, int>();
            foreach (var r in resources)
                if (counters.Inc(r.TypeName) <= amount)
                    yield return r;
        }

        public static int Inc<T>(this Dictionary<T, int> dictionary, T key)
        {
            if (dictionary.ContainsKey(key))
                return ++dictionary[key];
            dictionary.Add(key, 1);
            return 1;
        }

        public static Bundle ToBundle(this IEnumerable<Resource> resources, Uri _base)
        {
            var bundle = new Bundle();
            foreach (var resource in resources)
                // Make sure that resources without id's are posted.
                if (resource.Id != null)
                    bundle.Append(Bundle.HTTPVerb.PUT, resource);
                else
                    bundle.Append(Bundle.HTTPVerb.POST, resource);

            return bundle;
        }
    }
}