#region Information

// Solution:  Spark
// Spark.Engine
// File:  SearchService.cs
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
    using System.Net;
    using Core;
    using Extensions;
    using FhirOnAzure.Core;
    using FhirOnAzure.Service;
    using Hl7.Fhir.Model;
    using Hl7.Fhir.Rest;

    public class SearchService : ISearchService, IServiceListener
    {
        private readonly IFhirModel fhirModel;
        private readonly ILocalhost localhost;
        private readonly IFhirIndex fhirIndex;
        private readonly IndexService indexService;

        public SearchService(ILocalhost localhost, IFhirModel fhirModel, IFhirIndex fhirIndex,
            IndexService indexService = null)
        {
            this.fhirModel = fhirModel;
            this.localhost = localhost;
            this.indexService = indexService;
            this.fhirIndex = fhirIndex;
        }

        public Snapshot GetSnapshot(string type, SearchParams searchCommand)
        {
            Validate.TypeName(type);
            var results = fhirIndex.Search(type, searchCommand);

            if (results.HasErrors)
                throw new SparkException(HttpStatusCode.BadRequest, results.Outcome);

            var builder = new UriBuilder(localhost.Uri(type))
            {
                Query = results.UsedParameters
            };
            var link = builder.Uri;

            var snapshot = CreateSnapshot(link, results, searchCommand);
            return snapshot;
        }

        public Snapshot GetSnapshotForEverything(IKey key)
        {
            var searchCommand = new SearchParams();
            if (string.IsNullOrEmpty(key.ResourceId) == false)
                searchCommand.Add("_id", key.ResourceId);
            var compartment = fhirModel.FindCompartmentInfo(key.TypeName);
            if (compartment != null)
                foreach (var ri in compartment.ReverseIncludes)
                    searchCommand.RevInclude.Add(ri);

            return GetSnapshot(key.TypeName, searchCommand);
        }

        public IKey FindSingle(string type, SearchParams searchCommand)
        {
            return Key.ParseOperationPath(GetSearchResults(type, searchCommand).Single());
        }

        public IKey FindSingleOrDefault(string type, SearchParams searchCommand)
        {
            var value = GetSearchResults(type, searchCommand).SingleOrDefault();
            return value != null ? Key.ParseOperationPath(value) : null;
        }

        public SearchResults GetSearchResults(string type, SearchParams searchCommand)
        {
            Validate.TypeName(type);
            var results = fhirIndex.Search(type, searchCommand);

            if (results.HasErrors)
                throw new SparkException(HttpStatusCode.BadRequest, results.Outcome);

            return results;
        }

        public void Inform(Uri location, Entry interaction)
        {
            if (indexService != null)
                indexService.Process(interaction);

            else if (fhirIndex != null)
                fhirIndex.Process(interaction);
        }

        private Snapshot CreateSnapshot(Uri selflink, IEnumerable<string> keys, SearchParams searchCommand)
        {
            var sort = GetFirstSort(searchCommand);

            var count = searchCommand.Count;
            if (count.HasValue)
                selflink = selflink.AddParam(SearchParams.SEARCH_PARAM_COUNT, count.ToString());

            if (searchCommand.Sort.Any())
                foreach (var tuple in searchCommand.Sort)
                    selflink = selflink.AddParam(SearchParams.SEARCH_PARAM_SORT,
                        string.Format("{0}:{1}", tuple.Item1, tuple.Item2 == SortOrder.Ascending ? "asc" : "desc"));

            if (searchCommand.Include.Any())
                selflink = selflink.AddParam(SearchParams.SEARCH_PARAM_INCLUDE, searchCommand.Include.ToArray());

            if (searchCommand.RevInclude.Any())
                selflink = selflink.AddParam(SearchParams.SEARCH_PARAM_REVINCLUDE, searchCommand.RevInclude.ToArray());

            return Snapshot.Create(Bundle.BundleType.Searchset, selflink, keys, sort, count, searchCommand.Include,
                searchCommand.RevInclude);
        }

        private static string GetFirstSort(SearchParams searchCommand)
        {
            string firstSort = null;
            if (searchCommand.Sort != null && searchCommand.Sort.Any())
                firstSort = searchCommand.Sort[0].Item1; //TODO: Support sortorder and multiple sort arguments.
            return firstSort;
        }
    }
}