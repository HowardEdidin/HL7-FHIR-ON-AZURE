#region Information

// Solution:  Spark
// Spark.Engine
// File:  ConditionalHeaderParameters.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:11 PM

#endregion

namespace FhirOnAzure.Engine.Core
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using Extensions;

    public class ConditionalHeaderParameters
    {
        public ConditionalHeaderParameters()
        {
        }

        public ConditionalHeaderParameters(HttpRequestMessage request)
        {
            IfNoneMatchTags = request.IfNoneMatch();
            IfModifiedSince = request.IfModifiedSince();
        }

        public IEnumerable<string> IfNoneMatchTags { get; set; }
        public DateTimeOffset? IfModifiedSince { get; set; }
    }
}