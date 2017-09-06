#region Information

// Solution:  Spark
// Spark.Engine
// File:  ResourceManipulationOperationFactory.cs
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
    using System.Net.Http;
    using Core;
    using Extensions;
    using Hl7.Fhir.Model;
    using Hl7.Fhir.Rest;

    public static partial class ResourceManipulationOperationFactory
    {
        private static readonly Dictionary<Bundle.HTTPVerb,
            Func<Resource, IKey, ISearchService, SearchParams, ResourceManipulationOperation>> builders;

        private static ISearchService searchService;

        static ResourceManipulationOperationFactory()
        {
            builders =
                new Dictionary<Bundle.HTTPVerb, Func<Resource, IKey, ISearchService, SearchParams,
                    ResourceManipulationOperation>>();
            builders.Add(Bundle.HTTPVerb.POST, CreatePost);
            builders.Add(Bundle.HTTPVerb.PUT, CreatePut);
            builders.Add(Bundle.HTTPVerb.DELETE, CreateDelete);
        }

        public static ResourceManipulationOperation CreatePost(Resource resource, IKey key,
            ISearchService service = null, SearchParams command = null)
        {
            searchService = service;
            return new PostManipulationOperation(resource, key, GetSearchResult(key, command), command);
        }

        private static SearchResults GetSearchResult(IKey key, SearchParams command = null)
        {
            if (command == null || command.Parameters.Count == 0)
                return null;
            if (command != null && searchService == null)
                throw new InvalidOperationException("Unallowed operation");
            return searchService.GetSearchResults(key.TypeName, command);
        }

        public static ResourceManipulationOperation CreatePut(Resource resource, IKey key,
            ISearchService service = null, SearchParams command = null)
        {
            searchService = service;
            return new PutManipulationOperation(resource, key, GetSearchResult(key, command), command);
        }

        public static ResourceManipulationOperation CreateDelete(IKey key, ISearchService service = null,
            SearchParams command = null)
        {
            searchService = service;
            return new DeleteManipulationOperation(null, key, GetSearchResult(key, command), command);
        }

        private static ResourceManipulationOperation CreateDelete(Resource resource, IKey key,
            ISearchService service = null, SearchParams command = null)
        {
            searchService = service;
            return new DeleteManipulationOperation(null, key, GetSearchResult(key, command), command);
        }

        public static ResourceManipulationOperation GetManipulationOperation(Bundle.EntryComponent entryComponent,
            ILocalhost localhost, ISearchService service = null)
        {
            searchService = service;
            var method = localhost.ExtrapolateMethod(entryComponent, null); //CCR: is key needed? Isn't method required?
            var key = localhost.ExtractKey(entryComponent);
            var searchUri = GetSearchUri(entryComponent, method);

            return builders[method](entryComponent.Resource, key, service,
                searchUri != null ? ParseQueryString(localhost, searchUri) : null);
        }

        private static Uri GetSearchUri(Bundle.EntryComponent entryComponent, Bundle.HTTPVerb method)
        {
            Uri searchUri = null;
            if (method == Bundle.HTTPVerb.POST)
                searchUri = PostManipulationOperation.ReadSearchUri(entryComponent);
            else if (method == Bundle.HTTPVerb.PUT)
                searchUri = PutManipulationOperation.ReadSearchUri(entryComponent);
            else if (method == Bundle.HTTPVerb.DELETE)
                searchUri = DeleteManipulationOperation.ReadSearchUri(entryComponent);
            return searchUri;
        }

        private static SearchParams ParseQueryString(ILocalhost localhost, Uri searchUri)
        {
            var absoluteUri = localhost.Absolute(searchUri);
            var keysCollection = absoluteUri.ParseQueryString();

            var searchValues =
                keysCollection.Keys.Cast<string>()
                    .Select(k => new Tuple<string, string>(k, keysCollection[k]));

            return SearchParams.FromUriParamList(searchValues);
        }
    }
}