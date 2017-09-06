#region Information

// Solution:  Spark
// Spark.Mongo
// File:  HistoryExtension.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:24 PM

#endregion

namespace FhirOnAzure.Mongo.Store.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Engine.Auxiliary;
    using Engine.Core;
    using Engine.Store.Interfaces;
    using FhirOnAzure.Store.Mongo;
    using Hl7.Fhir.Model;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;
    using Query = MongoDB.Driver.Builders.Query;

    public class HistoryStore : IHistoryStore
    {
        private readonly MongoCollection<BsonDocument> _collection;

        public HistoryStore(string mongoUrl)
        {
            var database = MongoDatabaseFactory.GetMongoDatabase(mongoUrl);
            _collection = database.GetCollection(Collection.RESOURCE);
        }

        public Snapshot History(HistoryParameters parameters)
        {
            var clauses = new List<IMongoQuery>();
            if (parameters.Since != null)
                clauses.Add(Query.GT(Field.WHEN, BsonDateTime.Create(parameters.Since)));

            return CreateSnapshot(FetchPrimaryKeys(clauses), parameters.Count);
        }

        public Snapshot History(string resource, HistoryParameters parameters)
        {
            var clauses = new List<IMongoQuery> {Query.EQ(Field.TYPENAME, resource)};

            if (parameters.Since != null)
                clauses.Add(Query.GT(Field.WHEN, BsonDateTime.Create(parameters.Since)));

            return CreateSnapshot(FetchPrimaryKeys(clauses), parameters.Count);
        }

        public Snapshot History(IKey key, HistoryParameters parameters)
        {
            var clauses = new List<IMongoQuery>
            {
                Query.EQ(Field.TYPENAME, key.TypeName),
                Query.EQ(Field.RESOURCEID, key.ResourceId)
            };
            if (parameters.Since != null)
                clauses.Add(Query.GT(Field.WHEN, BsonDateTime.Create(parameters.Since)));

            return CreateSnapshot(FetchPrimaryKeys(clauses), parameters.Count);
        }

        private Snapshot CreateSnapshot(IEnumerable<string> keys, int? count = null, IList<string> includes = null,
            IList<string> reverseIncludes = null)
        {
            var link = new Uri(RestOperation.HISTORY, UriKind.Relative);
            var snapshot = Snapshot.Create(Bundle.BundleType.History, link, keys, "history", count, includes,
                reverseIncludes);
            return snapshot;
        }

        public IList<string> FetchPrimaryKeys(IMongoQuery query)
        {
            var cursor = _collection.Find(query)
                .SetSortOrder(SortBy.Descending(Field.WHEN));
            cursor = cursor.SetFields(Fields.Include(Field.PRIMARYKEY));

            return cursor.Select(doc => doc.GetValue(Field.PRIMARYKEY).AsString).ToList();
        }

        public IList<string> FetchPrimaryKeys(IEnumerable<IMongoQuery> clauses)
        {
            var mongoQueries = clauses as IMongoQuery[] ?? clauses.ToArray();
            var query = mongoQueries.Any() ? Query.And(mongoQueries) : Query.Empty;
            return FetchPrimaryKeys(query);
        }
    }
}