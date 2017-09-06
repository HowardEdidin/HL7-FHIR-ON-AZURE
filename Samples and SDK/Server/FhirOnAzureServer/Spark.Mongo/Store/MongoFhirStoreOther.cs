#region Information

// Solution:  Spark
// Spark.Mongo
// File:  MongoFhirStoreOther.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:24 PM

#endregion

namespace FhirOnAzure.Store.Mongo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Engine.Core;
    using Engine.Store.Interfaces;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;

    //TODO: decide if we still need this
    public class MongoFhirStoreOther
    {
        private readonly IFhirStore _mongoFhirStoreOther;
        private readonly MongoCollection<BsonDocument> collection;
        private readonly MongoDatabase database;

        public MongoFhirStoreOther(string mongoUrl, IFhirStore mongoFhirStoreOther)
        {
            _mongoFhirStoreOther = mongoFhirStoreOther;
            database = MongoDatabaseFactory.GetMongoDatabase(mongoUrl);
            collection = database.GetCollection(Collection.RESOURCE);
            //this.transaction = new MongoSimpleTransaction(collection);
        }

        //TODO: I've commented this. Do we still need it?
        //public IList<string> List(string resource, DateTimeOffset? since = null)
        //{
        //    var clauses = new List<IMongoQuery>();

        //    clauses.Add(MongoDB.Driver.Builders.Query.EQ(Field.TYPENAME, resource));
        //    if (since != null)
        //    {
        //        clauses.Add(MongoDB.Driver.Builders.Query.GT(Field.WHEN, BsonDateTime.Create(since)));
        //    }
        //    clauses.Add(MongoDB.Driver.Builders.Query.EQ(Field.STATE, Value.CURRENT));

        //    return FetchPrimaryKeys(clauses);
        //}


        public bool Exists(IKey key)
        {
            // PERF: efficiency
            var existing = _mongoFhirStoreOther.Get(key);
            return existing != null;
        }

        //public Interaction Get(string primarykey)
        //{
        //    IMongoQuery query = MonQ.Query.EQ(Field.PRIMARYKEY, primarykey);
        //    BsonDocument document = collection.FindOne(query);
        //    if (document != null)
        //    {
        //        Interaction entry = document.ToInteraction();
        //        return entry;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        public IList<Entry> GetCurrent(IEnumerable<string> identifiers, string sortby = null)
        {
            var clauses = new List<IMongoQuery>();
            var ids = identifiers.Select(i => (BsonValue) i);

            clauses.Add(Query.In(Field.REFERENCE, ids));
            clauses.Add(Query.EQ(Field.STATE, Value.CURRENT));
            var query = Query.And(clauses);

            var cursor = collection.Find(query);

            if (sortby != null)
                cursor = cursor.SetSortOrder(SortBy.Ascending(sortby));
            else
                cursor = cursor.SetSortOrder(SortBy.Descending(Field.WHEN));

            return cursor.ToEntries().ToList();
        }

        private void Supercede(IEnumerable<IKey> keys)
        {
            var pks = keys.Select(k => k.ToBsonReferenceKey());
            var query = Query.And(
                Query.In(Field.REFERENCE, pks),
                Query.EQ(Field.STATE, Value.CURRENT)
            );
            IMongoUpdate update = new UpdateDocument("$set",
                new BsonDocument
                {
                    {Field.STATE, Value.SUPERCEDED}
                }
            );
            collection.Update(query, update);
        }

        public void Add(IEnumerable<Entry> entries)
        {
            var enumerable = entries as Entry[] ?? entries.ToArray();
            var keys = enumerable.Select(i => i.Key);
            Supercede(keys);
            IList<BsonDocument> documents = enumerable.Select(SparkBsonHelper.ToBsonDocument).ToList();
            collection.InsertBatch(documents);
        }

        public void Replace(Entry entry)
        {
            var versionid = entry.Resource.Meta.VersionId;

            var query = Query.EQ(Field.VERSIONID, versionid);
            var current = collection.FindOne(query);
            var replacement = entry.ToBsonDocument();
            SparkBsonHelper.TransferMetadata(current, replacement);

            var update = Update.Replace(replacement);
            collection.Update(query, update);
        }

        public bool CustomResourceIdAllowed(string value)
        {
            if (value.StartsWith(Value.IDPREFIX))
            {
                var remainder = value.Substring(1);
                int i;
                var isint = int.TryParse(remainder, out i);
                return !isint;
            }
            return true;
        }

        //public Tag BsonValueToTag(BsonValue item)
        //{
        //    var tag = new Tag(
        //           item["term"].AsString,
        //           new Uri(item["scheme"].AsString),
        //           item["label"].AsString);

        //    return tag;
        //}

        //public IEnumerable<Tag> Tags()
        //{
        //    return collection.Distinct(Field.CATEGORY).Select(BsonValueToTag);
        //}

        //public IEnumerable<Tag> Tags(string resourcetype)
        //{
        //    IMongoQuery query = MonQ.Query.EQ(Field.COLLECTION, resourcetype);
        //    return collection.Distinct(Field.CATEGORY, query).Select(BsonValueToTag);
        //}

        //public IEnumerable<Uri> Find(params Tag[] tags)
        //{
        //    throw new NotImplementedException("Finding tags is not implemented on database level");
        //}

    }
}