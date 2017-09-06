#region Information

// Solution:  Spark
// Spark.Engine
// File:  SingleLocalhost.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:12 PM

#endregion

namespace FhirOnAzure.Engine.Core
{
    using System;

    public class Localhost : ILocalhost
    {
        public Localhost(Uri baseuri)
        {
            DefaultBase = baseuri;
        }

        public Uri DefaultBase { get; set; }

        public Uri Absolute(Uri uri)
        {
            if (uri.IsAbsoluteUri)
                return uri;
            var _base = DefaultBase.ToString().TrimEnd('/') + "/";
            var path = uri.ToString();
            return new Uri(_base + uri);
        }

        public bool IsBaseOf(Uri uri)
        {
            if (uri.IsAbsoluteUri)
            {
                var isbase = DefaultBase.Bugfixed_IsBaseOf(uri);
                return isbase;
            }
            return false;
        }

        public Uri GetBaseOf(Uri uri)
        {
            return IsBaseOf(uri) ? DefaultBase : null;
        }
    }
}