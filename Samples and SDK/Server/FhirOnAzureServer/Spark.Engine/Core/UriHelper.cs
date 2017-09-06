#region Information

// Solution:  Spark
// Spark.Engine
// File:  UriHelper.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:12 PM

#endregion

namespace FhirOnAzure.Engine.Core
{
    using System;
    using FhirOnAzure.Core;
    using Hl7.Fhir.Rest;

    public static class UriHelper
    {
        public const string CID = "cid";

        public static string CreateCID()
        {
            return string.Format("{0}:{1}", CID, Guid.NewGuid());
        }

        public static Uri CreateUrn()
        {
            return new Uri("urn:guid:" + Guid.NewGuid());
        }

        public static Uri CreateUuid()
        {
            return new Uri("urn:uuid:" + Guid.NewGuid());
        }

        public static bool IsHttpScheme(Uri uri)
        {
            if (uri != null)
                if (uri.IsAbsoluteUri)
                    return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
            return false;
        }

        public static bool IsTemporaryUri(this Uri uri)
        {
            if (uri == null) return false;

            return IsTemporaryUri(uri.ToString());
        }

        public static bool IsTemporaryUri(string uri)
        {
            return uri.StartsWith("urn:uuid:")
                   || uri.StartsWith("urn:guid:")
                   || uri.StartsWith("cid:");
        }


        /// <summary>
        ///     Determines wether the uri contains a hash (#) frament.
        /// </summary>
        public static bool HasFragment(this Uri uri)
        {
            if (uri.IsAbsoluteUri)
            {
                var fragment = uri.Fragment;
                return !string.IsNullOrEmpty(fragment);
            }
            var s = uri.ToString();
            return s.StartsWith("#");
        }

        public static Uri HistoryKeyFor(this IGenerator generator, Uri key)
        {
            var identity = new ResourceIdentity(key);
            var vid = generator.NextVersionId(identity.ResourceType);
            Uri result = identity.WithVersion(vid);
            return result;
        }

        /// <summary>
        ///     Bugfixed_IsBaseOf is a fix for Uri.IsBaseOf which has a bug
        /// </summary>
        public static bool Bugfixed_IsBaseOf(this Uri _base, Uri uri)
        {
            var b = _base.ToString().ToLowerInvariant();
            var u = uri.ToString().ToLowerInvariant();

            var isbase = u.StartsWith(b);
            return isbase;
        }
    }
}