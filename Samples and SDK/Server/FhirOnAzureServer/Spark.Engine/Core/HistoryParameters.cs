#region Information

// Solution:  Spark
// Spark.Engine
// File:  HistoryParameters.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:12 PM

#endregion

namespace FhirOnAzure.Engine.Core
{
    using System;
    using System.Net.Http;
    using Extensions;

    public class HistoryParameters
    {
        public HistoryParameters()
        {
        }

        public HistoryParameters(HttpRequestMessage request)
        {
            Count = request.GetIntParameter(FhirParameter.COUNT);
            Since = request.GetDateParameter(FhirParameter.SINCE);
            SortBy = request.GetParameter(FhirParameter.SORT);
        }

        public int? Count { get; set; }
        public DateTimeOffset? Since { get; set; }
        public string Format { get; set; }
        public string SortBy { get; set; }
    }
}