#region Information

// Solution:  Spark
// Spark.Mongo
// File:  MongoStoreBuilder.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:24 PM

#endregion

namespace FhirOnAzure.Mongo.Store
{
    using System;
    using Core;
    using Engine.Core;
    using Engine.Store.Interfaces;
    using Extensions;
    using FhirOnAzure.Search.Mongo;
    using FhirOnAzure.Store.Mongo;
    using Search.Common;
    using Search.Indexer;

    public class MongoStoreBuilder : IStorageBuilder
    {
        private readonly ILocalhost localhost;
        private readonly string mongoUrl;

        public MongoStoreBuilder(string mongoUrl, ILocalhost localhost)
        {
            this.mongoUrl = mongoUrl;
            this.localhost = localhost;
        }

        public T GetStore<T>()
        {
            throw new NotImplementedException();
        }

        public IFhirStore GetStore()
        {
            return new MongoFhirStore(mongoUrl);
        }

        public IHistoryStore GetHistoryStore()
        {
            return new HistoryStore(mongoUrl);
        }

        public IIndexStore GetIndexStore()
        {
            return new MongoIndexStore(mongoUrl, new MongoIndexMapper());
        }

        public IFhirIndex GetFhirIndex()
        {
            var indexStore = new MongoIndexStore(mongoUrl, new MongoIndexMapper());
            return new MongoFhirIndex(indexStore, new MongoIndexer(indexStore, new Definitions()),
                new MongoSearcher(indexStore, localhost, new FhirModel()));
        }

        public ISnapshotStore GeSnapshotStore()
        {
            return new MongoSnapshotStore(mongoUrl);
        }
    }
}