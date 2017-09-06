#region Information

// Solution:  Spark
// Spark.Engine
// File:  Snapshot.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:12 PM

#endregion

namespace FhirOnAzure.Engine.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Hl7.Fhir.Model;

    public class Snapshot
    {
        public const int NOCOUNT = -1;
        public const int MAX_PAGE_SIZE = 100;
        public ICollection<string> Includes;
        public ICollection<string> ReverseIncludes;
        public DateTimeOffset WhenCreated;


        public string Id { get; set; }
        public Bundle.BundleType Type { get; set; }

        public IEnumerable<string> Keys { get; set; }

        //public string FeedTitle { get; set; }
        public string FeedSelfLink { get; set; }

        public int Count { get; set; }
        public int? CountParam { get; set; }
        public string SortBy { get; set; }

        public static Snapshot Create(Bundle.BundleType type, Uri selflink, IEnumerable<string> keys, string sortby,
            int? count, IList<string> includes, IList<string> reverseIncludes)
        {
            var snapshot = new Snapshot();
            snapshot.Type = type;
            snapshot.Id = CreateKey();
            snapshot.WhenCreated = DateTimeOffset.UtcNow;
            snapshot.FeedSelfLink = selflink.ToString();

            snapshot.Includes = includes;
            snapshot.ReverseIncludes = reverseIncludes;
            snapshot.Keys = keys;
            snapshot.Count = keys.Count();
            snapshot.CountParam = NormalizeCount(count);

            snapshot.SortBy = sortby;
            return snapshot;
        }

        private static int? NormalizeCount(int? count)
        {
            if (count.HasValue)
                return Math.Min(count.Value, MAX_PAGE_SIZE);
            return count;
        }


        public static string CreateKey()
        {
            return Guid.NewGuid().ToString();
        }

        public bool InRange(int index)
        {
            if (index == 0 && Keys.Count() == 0)
                return true;

            var last = Keys.Count() - 1;
            return index > 0 || index <= last;
        }
    }

    public static class SnapshotExtensions
    {
        public static IEnumerable<string> Keys(this Bundle bundle)
        {
            return bundle.GetResources().Keys();
        }

        public static IEnumerable<string> Keys(this IEnumerable<Resource> resources)
        {
            return resources.Select(e => e.VersionId);
        }
    }
}