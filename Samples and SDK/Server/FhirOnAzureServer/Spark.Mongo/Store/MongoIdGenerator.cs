#region Information

// Solution:  Spark
// Spark.Mongo
// File:  MongoIdGenerator.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:24 PM

#endregion

namespace FhirOnAzure.Mongo.Store
{
    using Core;
    using FhirOnAzure.Store.Mongo;
    using Hl7.Fhir.Model;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;
    using Query = MongoDB.Driver.Builders.Query;

    public class MongoIdGenerator : IGenerator
    {
        private readonly MongoDatabase database;

        public MongoIdGenerator(string mongoUrl)
        {
            database = MongoDatabaseFactory.GetMongoDatabase(mongoUrl);
        }

        string IGenerator.NextResourceId(Resource resource)
        {
            var id = Next(resource.TypeName);
            return string.Format(Format.RESOURCEID, id);
        }

        string IGenerator.NextVersionId(string resource)
        {
            var name = resource + "_history";
            var id = Next(name);
            return string.Format(Format.VERSIONID, id);
        }

        string IGenerator.NextVersionId(string resourceType, string resourceIdentifier)
        {
            return ((IGenerator) this).NextVersionId(resourceType);
        }

        public string Next(string name)
        {
            var collection = database.GetCollection(Collection.COUNTERS);

            var args = new FindAndModifyArgs
            {
                Query = Query.EQ("_id", name),
                Update = Update.Inc(Field.COUNTERVALUE, 1),
                Fields = Fields.Include(Field.COUNTERVALUE),
                Upsert = true,
                VersionReturned = FindAndModifyDocumentVersion.Modified
            };

            var result = collection.FindAndModify(args);
            var document = result.ModifiedDocument;

            var value = document[Field.COUNTERVALUE].AsInt32.ToString();
            return value;
        }

        public static class Format
        {
            public static string RESOURCEID = "azure{0}";
            public static string VERSIONID = "azure{0}";
        }
    }
}