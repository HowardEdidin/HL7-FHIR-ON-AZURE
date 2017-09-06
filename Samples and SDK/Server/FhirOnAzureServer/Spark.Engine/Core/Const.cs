#region Information

// Solution:  Spark
// Spark.Engine
// File:  Const.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:11 PM

#endregion

namespace FhirOnAzure.Engine.Core
{
    public static class FhirRestOp
    {
        public const string SNAPSHOT = "_snapshot";
    }

    public static class FhirHeader
    {
        public const string CATEGORY = "Category";
    }

    public static class FhirParameter
    {
        public const string SNAPSHOT_ID = "id";
        public const string SNAPSHOT_INDEX = "start";
        public const string SUMMARY = "_summary";
        public const string COUNT = "_count";
        public const string SINCE = "_since";
        public const string SORT = "_sort";
    }
}