﻿using MongoDB.Bson;
using MongoDB.Driver;
using FhirOnAzure.Engine.Core;
using System;
using FhirOnAzure.Store.Mongo;
using FhirOnAzure.Engine.Model;
using FhirOnAzure.Mongo.Search.Indexer;
using FhirOnAzure.Engine.Store.Interfaces;

namespace FhirOnAzure.Mongo.Search.Common
{
    public class MongoIndexStore : IIndexStore
    {
        private MongoDatabase _database;
        private MongoIndexMapper _indexMapper;
        public MongoCollection<BsonDocument> Collection;

        public MongoIndexStore(string mongoUrl, MongoIndexMapper indexMapper)
        {
            _database = MongoDatabaseFactory.GetMongoDatabase(mongoUrl);
            _indexMapper = indexMapper; 
            Collection = _database.GetCollection(Config.MONGOINDEXCOLLECTION);
        }

        public void Save(IndexValue indexValue)
        {
            var result = _indexMapper.MapEntry(indexValue);

            foreach (var doc in result)
            {
                Save(doc);
            }
        }

        public void Save(BsonDocument document)
        {
            try
            {
                string keyvalue = document.GetValue(InternalField.ID).ToString();
                IMongoQuery query = MongoDB.Driver.Builders.Query.EQ(InternalField.ID, keyvalue);

                // todo: should use Update: collection.Update();
                Collection.Remove(query);
                Collection.Insert(document);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public void Delete(Entry entry)
        {
            string id = entry.Key.WithoutVersion().ToOperationPath();
            IMongoQuery query = MongoDB.Driver.Builders.Query.EQ(InternalField.ID, id);
            Collection.Remove(query);
        }

        public void Clean()
        {
            Collection.RemoveAll();
        }

    }
}
