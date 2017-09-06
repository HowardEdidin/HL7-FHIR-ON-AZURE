#region Information

// Solution:  Spark
// Spark.Mongo
// File:  MongoDatabaseFactory.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  09/06/2017 : 11:12 AM

#endregion

namespace FhirOnAzure.Store.Mongo
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Authentication;
    using MongoDB.Driver;

    public static class MongoDatabaseFactory
    {
        private static Dictionary<string, MongoDatabase> _instances;

        public static MongoDatabase GetMongoDatabase(string url)
        {
            if (_instances == null) //instances dictionary is not at all initialized
                _instances = new Dictionary<string, MongoDatabase>();
            if (_instances.Any(i => i.Key == url))
                return _instances.First(i => i.Key == url).Value; //now there must be one.
            var result = CreateMongoDatabase();
            _instances.Add(url, result);
            return _instances.First(i => i.Key == url).Value; //now there must be one.
        }

        private static MongoDatabase CreateMongoDatabase()
        {
            const string connectionString =
                @"mongodb://mongofhir:2sqm6y5HN5e4yoPhCRMSUUKxlANMgJPX91L52vstD6PGxr2zlFYBHanq6bkZHtcTipZ1TRvytSxY5ZMriLTjKQ==@mongofhir.documents.azure.com:10255/?ssl=true&replicaSet=globaldb";

            var settings = MongoClientSettings.FromUrl(
                new MongoUrl(connectionString)
            );
            settings.SslSettings =
                new SslSettings {EnabledSslProtocols = SslProtocols.Tls12};
            var mongoClient = new MongoClient(settings);


#pragma warning disable 618
            return mongoClient.GetServer().GetDatabase("admin");
#pragma warning restore 618
        }
    }
}