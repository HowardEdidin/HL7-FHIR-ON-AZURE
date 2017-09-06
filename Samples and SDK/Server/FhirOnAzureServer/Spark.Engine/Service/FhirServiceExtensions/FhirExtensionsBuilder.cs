#region Information

// Solution:  Spark
// Spark.Engine
// File:  FhirExtensionsBuilder.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:53 PM

#endregion

namespace FhirOnAzure.Engine.Service.FhirServiceExtensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using FhirOnAzure.Core;
    using FhirOnAzure.Service;
    using Store.Interfaces;

    public class FhirExtensionsBuilder : IFhirExtensionsBuilder
    {
        private readonly Uri baseUri;
        private readonly IList<IFhirServiceExtension> extensions;
        private readonly IStorageBuilder fhirStoreBuilder;

        public FhirExtensionsBuilder(IStorageBuilder fhirStoreBuilder, Uri baseUri)
        {
            this.fhirStoreBuilder = fhirStoreBuilder;
            this.baseUri = baseUri;
            var extensionBuilders = new Func<IFhirServiceExtension>[]
            {
                GetSearch,
                GetHistory,
                GetCapabilityStatement,
                GetPaging,
                GetStorage
            };
            extensions = extensionBuilders.Select(builder => builder()).Where(ext => ext != null).ToList();
        }

        public IEnumerable<IFhirServiceExtension> GetExtensions()
        {
            return extensions;
        }

        public IEnumerator<IFhirServiceExtension> GetEnumerator()
        {
            return extensions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected virtual IFhirServiceExtension GetSearch()
        {
            var fhirStore = fhirStoreBuilder.GetStore<IFhirIndex>();
            if (fhirStore != null)
                return new SearchService(new Localhost(baseUri), new FhirModel(), fhirStore);
            return null;
        }

        protected virtual IFhirServiceExtension GetHistory()
        {
            var historyStore = fhirStoreBuilder.GetStore<IHistoryStore>();
            if (historyStore != null)
                return new HistoryService(historyStore);
            return null;
        }

        protected virtual IFhirServiceExtension GetCapabilityStatement()
        {
            return new CapabilityStatementService(new Localhost(baseUri));
        }


        protected virtual IFhirServiceExtension GetPaging()
        {
            var fhirStore = fhirStoreBuilder.GetStore<IFhirStore>();
            var snapshotStore = fhirStoreBuilder.GetStore<ISnapshotStore>();
            var storeGenerator = fhirStoreBuilder.GetStore<IGenerator>();
            if (fhirStore != null)
                return new PagingService(snapshotStore,
                    new SnapshotPaginationProvider(fhirStore, new Transfer(storeGenerator, new Localhost(baseUri)),
                        new Localhost(baseUri), new SnapshotPaginationCalculator()));
            return null;
        }

        protected virtual IFhirServiceExtension GetStorage()
        {
            var fhirStore = fhirStoreBuilder.GetStore<IFhirStore>();
            var fhirGenerator = fhirStoreBuilder.GetStore<IGenerator>();
            if (fhirStore != null)
                return new ResourceStorageService(new Transfer(fhirGenerator, new Localhost(baseUri)), fhirStore);
            return null;
        }
    }
}