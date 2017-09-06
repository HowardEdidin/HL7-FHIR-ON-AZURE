#region Information

// Solution:  Spark
// Spark.Mongo
// File:  Transaction.cs
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
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;

    public class MongoTransaction
    {
        private readonly MongoCollection<BsonDocument> collection;
        private string transid;

        public MongoTransaction(MongoCollection<BsonDocument> collection)
        {
            this.collection = collection;
        }

        public IEnumerable<BsonValue> KeysOf(IEnumerable<BsonDocument> documents)
        {
            foreach (var document in documents)
            {
                BsonValue value = null;
                if (document.TryGetValue(Field.RESOURCEID, out value))
                    yield return value;
            }
        }

        public BsonValue KeyOf(BsonDocument document)
        {
            BsonValue value = null;
            if (document.TryGetValue(Field.RESOURCEID, out value))
                return value;
            return null;
        }

        private void MarkExisting(BsonDocument document)
        {
            var id = document.GetValue(Field.RESOURCEID);
            var query = Query.And(Query.EQ(Field.RESOURCEID, id), Query.EQ(Field.STATE, Value.CURRENT));
            IMongoUpdate update = new UpdateDocument("$set",
                new BsonDocument
                {
                    {Field.TRANSACTION, transid}
                }
            );
            collection.Update(query, update, UpdateFlags.Multi);
        }

        public void MarkExisting(IEnumerable<BsonDocument> documents)
        {
            var keys = KeysOf(documents);
            IMongoUpdate update = new UpdateDocument("$set",
                new BsonDocument
                {
                    {Field.TRANSACTION, transid}
                }
            );
            var query = Query.And(Query.EQ(Field.STATE, Value.CURRENT), Query.In(Field.RESOURCEID, keys));
            collection.Update(query, update, UpdateFlags.Multi);
        }

        public void RemoveQueued(string transid)
        {
            var query = Query.And(
                Query.EQ(Field.TRANSACTION, transid),
                Query.EQ(Field.STATE, Value.QUEUED)
            );
            collection.Remove(query);
        }

        public void RemoveTransaction(string transid)
        {
            var query = Query.EQ(Field.TRANSACTION, transid);
            IMongoUpdate update = new UpdateDocument("$set",
                new BsonDocument
                {
                    {Field.TRANSACTION, 0}
                }
            );
            collection.Update(query, update, UpdateFlags.Multi);
        }

        private void PrepareNew(BsonDocument document)
        {
            //document.Remove(Field.RecordId); voor Fhir-documenten niet nodig
            document.Set(Field.TRANSACTION, transid);
            document.Set(Field.STATE, Value.QUEUED);
        }

        private void PrepareNew(IEnumerable<BsonDocument> documents)
        {
            foreach (var doc in documents)
                PrepareNew(doc);
        }

        private void BulkUpdateStatus(string transid, string statusfrom, string statusto)
        {
            var query = Query.And(Query.EQ(Field.TRANSACTION, transid), Query.EQ(Field.STATE, statusfrom));
            IMongoUpdate update = new UpdateDocument("$set",
                new BsonDocument
                {
                    {Field.STATE, statusto}
                }
            );
            collection.Update(query, update, UpdateFlags.Multi);
        }

        public void Begin()
        {
            transid = Guid.NewGuid().ToString();
        }

        public void Rollback()
        {
            RemoveQueued(transid);
            RemoveTransaction(transid);
        }

        public void Commit()
        {
            BulkUpdateStatus(transid, Value.CURRENT, Value.SUPERCEDED);
            BulkUpdateStatus(transid, Value.QUEUED, Value.CURRENT);
        }

        public void Insert(BsonDocument document)
        {
            MarkExisting(document);
            PrepareNew(document);
            collection.Save(document);
        }

        public void InsertBatch(IList<BsonDocument> documents)
        {
            MarkExisting(documents);
            PrepareNew(documents);
            collection.InsertBatch(documents);
        }

        public BsonDocument ReadCurrent(string resourceid)
        {
            var query =
                Query.And(
                    Query.EQ(Field.RESOURCEID, resourceid),
                    Query.EQ(Field.STATE, Value.CURRENT)
                );
            return collection.FindOne(query);
        }
    }
}