#region Information

// Solution:  Spark
// Spark.Mongo
// File:  BsonHelper.cs
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
    using Engine.Core;
    using Hl7.Fhir.Model;
    using Hl7.Fhir.Serialization;
    using MongoDB.Bson;
    using MongoDB.Driver;

    public static class SparkBsonHelper
    {
        public static BsonDocument CreateDocument(Resource resource)
        {
            if (resource != null)
            {
                var json = FhirSerializer.SerializeResourceToJson(resource);
                return BsonDocument.Parse(json);
            }
            return new BsonDocument();
        }

        public static BsonValue ToBsonReferenceKey(this IKey key)
        {
            return new BsonString(key.TypeName + "/" + key.ResourceId);
        }

        public static BsonDocument ToBsonDocument(this Entry entry)
        {
            var document = CreateDocument(entry.Resource);
            AddMetaData(document, entry);
            return document;
        }

        public static Resource ParseResource(BsonDocument document)
        {
            RemoveMetadata(document);
            var json = document.ToJson();
            var resource = FhirParser.ParseResourceFromJson(json);
            return resource;
        }

        public static Entry ExtractMetadata(BsonDocument document)
        {
            var when = GetVersionDate(document);
            var key = GetKey(document);
            var method = (Bundle.HTTPVerb) (int) document[Field.METHOD];

            RemoveMetadata(document);
            return Entry.Create(method, key, when);
        }

        public static Entry ToEntry(this BsonDocument document)
        {
            if (document == null) return null;

            try
            {
                var entry = ExtractMetadata(document);
                if (entry.IsPresent)
                    entry.Resource = ParseResource(document);
                return entry;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Mongo document contains invalid BSON to parse.", e);
            }
        }

        public static IEnumerable<Entry> ToEntries(this MongoCursor<BsonDocument> cursor)
        {
            foreach (var document in cursor)
            {
                var entry = document.ToEntry();
                yield return entry;
            }
        }

        public static DateTime GetVersionDate(BsonDocument document)
        {
            var value = document[Field.WHEN];
            return value.ToUniversalTime();
        }

        private static void EnsureMeta(Resource resource)
        {
            if (resource.Meta == null)
                resource.Meta = new Meta();
        }

        public static void AddVersionDate(Entry entry, DateTime when)
        {
            entry.When = when;
            if (entry.Resource != null)
            {
                EnsureMeta(entry.Resource);
                entry.Resource.Meta.LastUpdated = when;
            }
        }

        public static void RemoveMetadata(BsonDocument document)
        {
            // This field gives an error if not removed. Not sure of its actual intention
            document.Remove(Field.RESOURCEID);

            document.Remove(Field.PRIMARYKEY);
            document.Remove(Field.REFERENCE);
            document.Remove(Field.WHEN);
            document.Remove(Field.STATE);
            document.Remove(Field.VERSIONID);
            document.Remove(Field.TYPENAME);
            document.Remove(Field.METHOD);
            document.Remove(Field.TRANSACTION);
        }

        public static void AddMetaData(BsonDocument document, Entry entry)
        {
            document[Field.METHOD] = entry.Method;
            document[Field.PRIMARYKEY] = entry.Key.ToOperationPath();
            document[Field.REFERENCE] = entry.Key.ToBsonReferenceKey();
            AddMetaData(document, entry.Key, entry.Resource);
        }

        private static void AssertKeyIsValid(IKey key)
        {
            var valid = key.Base == null && key.TypeName != null && key.ResourceId != null && key.VersionId != null;
            if (!valid)
                throw new Exception("This key is not valid for storage: " + key);
        }

        public static void AddMetaData(BsonDocument document, IKey key, Resource resource)
        {
            AssertKeyIsValid(key);
            document[Field.TYPENAME] = key.TypeName;
            document[Field.RESOURCEID] = key.ResourceId;
            document[Field.VERSIONID] = key.VersionId;

            document[Field.WHEN] = resource != null && resource.Meta != null && resource.Meta.LastUpdated.HasValue
                ? resource.Meta.LastUpdated.Value.UtcDateTime
                : DateTime.UtcNow;
            document[Field.STATE] = Value.CURRENT;
        }

        public static IKey GetKey(BsonDocument document)
        {
            var key = new Key
            {
                TypeName = (string)document[Field.TYPENAME],
                ResourceId = (string)document[Field.RESOURCEID],
                VersionId = (string)document[Field.VERSIONID]
            };

            return key;
        }

        public static void TransferMetadata(BsonDocument from, BsonDocument to)
        {
            to[Field.TYPENAME] = from[Field.TYPENAME];
            to[Field.RESOURCEID] = from[Field.RESOURCEID];
            to[Field.VERSIONID] = from[Field.VERSIONID];
            to[Field.WHEN] = from[Field.WHEN];
            to[Field.METHOD] = from[Field.METHOD];
            to[Field.STATE] = from[Field.STATE];
        }
    }
}