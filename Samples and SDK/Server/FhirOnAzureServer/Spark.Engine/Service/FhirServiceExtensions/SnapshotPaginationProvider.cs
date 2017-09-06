#region Information

// Solution:  Spark
// Spark.Engine
// File:  SnapshotPaginationProvider.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:53 PM

#endregion

namespace FhirOnAzure.Engine.Service.FhirServiceExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Extensions;
    using FhirOnAzure.Core;
    using FhirOnAzure.Service;
    using Hl7.Fhir.Model;
    using Store.Interfaces;

    public class SnapshotPaginationProvider : ISnapshotPaginationProvider, ISnapshotPagination
    {
        private readonly ISnapshotPaginationCalculator _snapshotPaginationCalculator;
        private readonly ILocalhost localhost;
        private readonly ITransfer transfer;
        private readonly IFhirStore fhirStore;
        private Snapshot snapshot;

        public SnapshotPaginationProvider(IFhirStore fhirStore, ITransfer transfer, ILocalhost localhost,
            ISnapshotPaginationCalculator snapshotPaginationCalculator)
        {
            this.fhirStore = fhirStore;
            this.transfer = transfer;
            this.localhost = localhost;
            _snapshotPaginationCalculator = snapshotPaginationCalculator;
        }

        public Bundle GetPage(int? index = null, Action<Entry> transformElement = null)
        {
            if (snapshot == null)
                throw Error.NotFound("There is no paged snapshot");

            if (!snapshot.InRange(index ?? 0))
                throw Error.NotFound(
                    "The specified index lies outside the range of available results ({0}) in snapshot {1}",
                    snapshot.Keys.Count(), snapshot.Id);

            return CreateBundle(index);
        }

        public ISnapshotPagination StartPagination(Snapshot snapshot)
        {
            this.snapshot = snapshot;
            return this;
        }

        private Bundle CreateBundle(int? start = null)
        {
            var bundle = new Bundle();
            bundle.Type = snapshot.Type;
            bundle.Total = snapshot.Count;
            bundle.Id = UriHelper.CreateUuid().ToString();

            var keys = _snapshotPaginationCalculator.GetKeysForPage(snapshot, start).ToList();
            IList<Entry> entries = fhirStore.Get(keys).ToList();
            if (snapshot.SortBy != null)
                entries = entries.Select(e => new {Entry = e, Index = keys.IndexOf(e.Key)})
                    .OrderBy(e => e.Index)
                    .Select(e => e.Entry).ToList();
            var included = GetIncludesRecursiveFor(entries, snapshot.Includes);
            entries.Append(included);

            transfer.Externalize(entries);
            bundle.Append(entries);
            BuildLinks(bundle, start);

            return bundle;
        }


        private IList<Entry> GetIncludesRecursiveFor(IList<Entry> entries, IEnumerable<string> includes)
        {
            IList<Entry> included = new List<Entry>();

            var latest = GetIncludesFor(entries, includes);
            int previouscount;
            do
            {
                previouscount = included.Count;
                included.AppendDistinct(latest);
                latest = GetIncludesFor(latest, includes);
            } while (included.Count > previouscount);
            return included;
        }

        private IList<Entry> GetIncludesFor(IList<Entry> entries, IEnumerable<string> includes)
        {
            if (includes == null) return new List<Entry>();

            var paths = includes.SelectMany(i => IncludeToPath(i));
            IList<IKey> identifiers = entries.GetResources().GetReferences(paths).Distinct()
                .Select(k => (IKey) Key.ParseOperationPath(k)).ToList();

            IList<Entry> result = fhirStore.Get(identifiers).ToList();

            return result;
        }

        private void BuildLinks(Bundle bundle, int? start = null)
        {
            bundle.SelfLink = start == null
                ? localhost.Absolute(new Uri(snapshot.FeedSelfLink, UriKind.RelativeOrAbsolute))
                : BuildSnapshotPageLink(0);
            bundle.FirstLink = BuildSnapshotPageLink(0);
            bundle.LastLink = BuildSnapshotPageLink(_snapshotPaginationCalculator.GetIndexForLastPage(snapshot));

            var previousPageIndex = _snapshotPaginationCalculator.GetIndexForPreviousPage(snapshot, start);
            if (previousPageIndex != null)
                bundle.PreviousLink = BuildSnapshotPageLink(previousPageIndex);

            var nextPageIndex = _snapshotPaginationCalculator.GetIndexForNextPage(snapshot, start);
            if (nextPageIndex != null)
                bundle.NextLink = BuildSnapshotPageLink(nextPageIndex);
        }

        private Uri BuildSnapshotPageLink(int? snapshotIndex)
        {
            if (snapshotIndex == null)
                return null;

            Uri baseurl;
            if (string.IsNullOrEmpty(snapshot.Id) == false)
                baseurl = new Uri(localhost.DefaultBase + "/" + FhirRestOp.SNAPSHOT)
                    .AddParam(FhirParameter.SNAPSHOT_ID, snapshot.Id);
            else
                baseurl = new Uri(snapshot.FeedSelfLink);

            return baseurl
                .AddParam(FhirParameter.SNAPSHOT_INDEX, snapshotIndex.ToString());
        }

        private IEnumerable<string> IncludeToPath(string include)
        {
            var _include = include.Split(':');
            var resource = _include.FirstOrDefault();
            var paramname = _include.Skip(1).FirstOrDefault();
            var param = ModelInfo.SearchParameters.FirstOrDefault(p => p.Resource == resource && p.Name == paramname);
            if (param != null)
                return param.Path;
            return Enumerable.Empty<string>();
        }
    }
}