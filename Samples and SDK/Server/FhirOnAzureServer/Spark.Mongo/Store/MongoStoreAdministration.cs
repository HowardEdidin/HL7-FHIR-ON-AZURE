#region Information

// Solution:  Spark
// Spark.Mongo
// File:  MongoStoreAdministration.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:24 PM

#endregion

namespace FhirOnAzure.Mongo.Store
{
    using System.Collections.Generic;
    using Engine.Interfaces;
    using FhirOnAzure.Store.Mongo;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;

    public class MongoStoreAdministration : IFhirStoreAdministration
    {
        private readonly MongoCollection<BsonDocument> collection;
        private readonly MongoDatabase database;

        public MongoStoreAdministration(string mongoUrl)
        {
            database = MongoDatabaseFactory.GetMongoDatabase(mongoUrl);
            collection = database.GetCollection(Collection.RESOURCE);
        }

        public void Clean()
        {
            EraseData();
            EnsureIndices();
        }

        // Drops all collections, including the special 'counters' collection for generating ids,
        // AND the binaries stored at Amazon S3
        private void EraseData()
        {
            // Don't try this at home
            var collectionsToDrop = new[] {Collection.RESOURCE, Collection.COUNTERS, Collection.SNAPSHOT};
            DropCollections(collectionsToDrop);

            /*
            // When using Amazon S3, remove blobs from there as well
            if (Config.Settings.UseS3)
            {
                using (var blobStorage = getBlobStorage())
                {
                    if (blobStorage != null)
                    {
                        blobStorage.Open();
                        blobStorage.DeleteAll();
                        blobStorage.Close();
                    }
                }
            }
            */
        }

        private void DropCollections(IEnumerable<string> collections)
        {
            foreach (var name in collections)
                TryDropCollection(name);
        }


        private void EnsureIndices()
        {
            collection.CreateIndex(Field.STATE, Field.METHOD, Field.TYPENAME);
            collection.CreateIndex(Field.PRIMARYKEY, Field.STATE);
            var index = IndexKeys.Descending(Field.WHEN).Ascending(Field.TYPENAME);
            collection.CreateIndex(index);
        }

        private void TryDropCollection(string name)
        {
            try
            {
                database.DropCollection(name);
            }
            catch
            {
                //don't worry. if it's not there. it's not there.
            }
        }
    }
}