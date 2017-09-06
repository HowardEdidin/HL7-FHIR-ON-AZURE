#region Information

// Solution:  Spark
// Spark.Mongo
// File:  MongoSnapshotStore.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:24 PM

#endregion

namespace FhirOnAzure.Mongo.Store
{
    using Engine.Core;
    using Engine.Store.Interfaces;
    using FhirOnAzure.Store.Mongo;
    using MongoDB.Driver;

    public class MongoSnapshotStore : ISnapshotStore
    {
        private readonly MongoDatabase database;

        public MongoSnapshotStore(string mongoUrl)
        {
            database = MongoDatabaseFactory.GetMongoDatabase(mongoUrl);
        }

        public void AddSnapshot(Snapshot snapshot)
        {
            var collection = database.GetCollection(Collection.SNAPSHOT);
            collection.Save(snapshot);
        }

        public Snapshot GetSnapshot(string snapshotid)
        {
            var collection = database.GetCollection(Collection.SNAPSHOT);
            return collection.FindOneByIdAs<Snapshot>(snapshotid);
        }
    }
}