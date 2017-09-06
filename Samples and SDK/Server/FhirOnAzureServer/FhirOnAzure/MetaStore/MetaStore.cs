/* 
 * Copyright (c) 2014, Furore (info@furore.com) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://raw.github.com/furore-fhir/spark/master/LICENSE
 */

namespace FhirOnAzure.MetaStore
{
    using System.Collections.Generic;
    using Hl7.Fhir.Model;
    using MongoDB.Driver;
    using Store.Mongo;
    using Query = MongoDB.Driver.Builders.Query;

    public class MetaContext
    {
        private readonly MongoCollection collection;
        private MongoDatabase db;

        public MetaContext(MongoDatabase db)
        {
            this.db = db;
            collection = db.GetCollection(Collection.RESOURCE);
        }

        public List<ResourceStat> GetResourceStats()
        {
            var stats = new List<ResourceStat>();
            var names = ModelInfo.SupportedResources;

            foreach (var name in names)
            {
                var query = Query.And(Query.EQ(Field.TYPENAME, name), Query.EQ(Field.STATE, Value.CURRENT));
                var count = collection.Count(query);
                stats.Add(new ResourceStat {ResourceName = name, Count = count});
            }
            return stats;
        }
    }
}