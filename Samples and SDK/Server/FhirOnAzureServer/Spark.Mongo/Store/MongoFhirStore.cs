#region Information

// Solution:  Spark
// Spark.Mongo
// File:  MongoFhirStore.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:24 PM

#endregion

using MonQ = MongoDB.Driver.Builders;


namespace FhirOnAzure.Store.Mongo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Engine.Core;
    using Engine.Store.Interfaces;
    using MongoDB.Bson;
    using MongoDB.Driver;

    public class MongoFhirStore : IFhirStore
    {
        private readonly MongoCollection<BsonDocument> _collection;

        public MongoFhirStore(string mongoUrl)
        {
            var database = MongoDatabaseFactory.GetMongoDatabase(mongoUrl);
            _collection = database.GetCollection(Collection.RESOURCE);
            //this.transaction = new MongoSimpleTransaction(collection);
        }

        public void Add(Entry entry)
        {
            var document = entry.ToBsonDocument();
            Supercede(entry.Key);
            _collection.Save(document);
        }

        public Entry Get(IKey key)
        {
            var clauses = new List<IMongoQuery>
            {
                MonQ.Query.EQ(Field.TYPENAME, key.TypeName),
                MonQ.Query.EQ(Field.RESOURCEID, key.ResourceId),
                key.HasVersionId()
                    ? MonQ.Query.EQ(Field.VERSIONID, key.VersionId)
                    : MonQ.Query.EQ(Field.STATE, Value.CURRENT)
            };


            var query = MonQ.Query.And(clauses);

            var document = _collection.FindOne(query);
            return document.ToEntry();
        }

        public IList<Entry> Get(IEnumerable<IKey> identifiers)
        {
            var keys = identifiers as IKey[] ?? identifiers.ToArray();
            if (!keys.Any())
                return new List<Entry>();

            IList<IKey> identifiersList = keys.ToList();
            var versionedIdentifiers = GetBsonValues(identifiersList, k => k.HasVersionId());
            var unversionedIdentifiers = GetBsonValues(identifiersList, k => k.HasVersionId() == false);

            var queries = new List<IMongoQuery>();
            var bsonValues = versionedIdentifiers as BsonValue[] ?? versionedIdentifiers.ToArray();
            if (bsonValues.Any())
                queries.Add(GetSpecificVersionQuery(bsonValues));
            var enumerable = unversionedIdentifiers as BsonValue[] ?? unversionedIdentifiers.ToArray();
            if (enumerable.Any())
                queries.Add(GetCurrentVersionQuery(enumerable));
            var query = MonQ.Query.Or(queries);

            var cursor = _collection.Find(query);

            return cursor.ToEntries().ToList();
        }

        private static IEnumerable<BsonValue> GetBsonValues(IEnumerable<IKey> identifiers, Func<IKey, bool> keyCondition)
        {
            return identifiers.Where(keyCondition).Select(k => (BsonValue) k.ToString());
        }

        private static IMongoQuery GetCurrentVersionQuery(IEnumerable<BsonValue> ids)
        {
            var clauses = new List<IMongoQuery>
            {
                MonQ.Query.In(Field.REFERENCE, ids),
                MonQ.Query.EQ(Field.STATE, Value.CURRENT)
            };
            return MonQ.Query.And(clauses);
        }

        private static IMongoQuery GetSpecificVersionQuery(IEnumerable<BsonValue> ids)
        {
            var clauses = new List<IMongoQuery>
            {
                MonQ.Query.In(Field.PRIMARYKEY, ids)
            };

            return MonQ.Query.And(clauses);
        }

        private void Supercede(IKey key)
        {
            var pk = key.ToBsonReferenceKey();
            var query = MonQ.Query.And(
                MonQ.Query.EQ(Field.REFERENCE, pk),
                MonQ.Query.EQ(Field.STATE, Value.CURRENT)
            );

            IMongoUpdate update = new UpdateDocument("$set",
                new BsonDocument
                {
                    {Field.STATE, Value.SUPERCEDED}
                }
            );
            _collection.Update(query, update);
        }
    }
}