#region Information

// Solution:  Spark
// Spark.Engine
// File:  IndexFieldNames.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:01 PM

#endregion

namespace FhirOnAzure.Engine.Search.Model
{
    public static class IndexFieldNames
    {
        public const string
            // Internally stored search fields 
            ID = "internal_id",
            JUSTID = "internal_justid",
            SELFLINK = "internal_selflink",
            CONTAINER = "internal_container",
            RESOURCE = "internal_resource",
            LEVEL = "internal_level",
            TAG = "internal_tag",
            TAGSCHEME = "scheme",
            TAGTERM = "term",
            TAGLABEL = "label",
            LASTUPDATED = "lastupdated";

        public static string[] All = {ID, JUSTID, SELFLINK, CONTAINER, RESOURCE, LEVEL, TAG, LASTUPDATED};
    }
}