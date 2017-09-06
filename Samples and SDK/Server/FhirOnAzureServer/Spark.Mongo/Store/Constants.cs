﻿#region Information

// Solution:  Spark
// Spark.Mongo
// File:  Constants.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:24 PM

#endregion

namespace FhirOnAzure.Store.Mongo
{
    public static class Collection
    {
        public const string RESOURCE = "resources";
        public const string COUNTERS = "counters";
        public const string SNAPSHOT = "snapshots";
    }

    public static class Field
    {
        // The id field is an actual field in the resource, so this const can't be changed.
        public const string RESOURCEID = "Id";

        public const string COUNTERVALUE = "last";
        public const string CATEGORY = "category";

        // Meta fields
        public const string PRIMARYKEY = "_id";

        // The current key is TYPENAME/ID for example: Patient/1
        // This is to be able to batch supercede a bundle of different resource types
        public const string REFERENCE = "@REFERENCE";

        public const string STATE = "@state";
        public const string WHEN = "@when";
        public const string METHOD = "@method"; // Present / Gone
        public const string TYPENAME = "@typename"; // Patient, Organization, etc.

        public const string VERSIONID = "@VersionId"
            ; // The resource versionid is in Resource.Meta. This is a administrative copy

        internal const string TRANSACTION = "@transaction";
        //internal const string TransactionState = "@transstate";
    }

    public static class Value
    {
        public const string CURRENT = "current";
        public const string SUPERCEDED = "superceded";
        internal const string QUEUED = "queued";
        public const string IDPREFIX = "nl.furore.spark.";
        public const string VIDPREFIX = "h";
    }
}