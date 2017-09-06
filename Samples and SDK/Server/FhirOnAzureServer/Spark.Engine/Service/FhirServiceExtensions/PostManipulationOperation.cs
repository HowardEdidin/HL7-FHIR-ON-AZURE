#region Information

// Solution:  Spark
// Spark.Engine
// File:  PostManipulationOperation.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:53 PM

#endregion

namespace FhirOnAzure.Engine.Service.FhirServiceExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Core;
    using Hl7.Fhir.Model;
    using Hl7.Fhir.Rest;

    public static partial class ResourceManipulationOperationFactory
    {
        private class PostManipulationOperation : ResourceManipulationOperation
        {
            public PostManipulationOperation(Resource resource, IKey operationKey, SearchResults searchResults,
                SearchParams searchCommand = null)
                : base(resource, operationKey, searchResults, searchCommand)
            {
            }

            public static Uri ReadSearchUri(Bundle.EntryComponent entry)
            {
                if (string.IsNullOrEmpty(entry.Request.IfNoneExist) == false)
                    return new Uri(string.Format("{0}?{1}", entry.TypeName, entry.Request.IfNoneExist),
                        UriKind.Relative);
                return null;
            }

            protected override IEnumerable<Entry> ComputeEntries()
            {
                Entry postEntry = null;
                if (SearchResults != null)
                {
                    if (SearchResults.Count > 1)
                        throw new SparkException(HttpStatusCode.PreconditionFailed,
                            string.Format(
                                "Multiple matches found when trying to resolve conditional create. Client's criteria were not selective enough.{0}",
                                GetSearchInformation()));
                    var localKeyValue = SearchResults.SingleOrDefault();
                    //throw exception. probably we should manually throw this in order to add fhir specific details
                    if (string.IsNullOrEmpty(localKeyValue) == false)
                    {
                        var localKey = Key.ParseOperationPath(localKeyValue);
                        postEntry = Entry.Create(Bundle.HTTPVerb.GET, localKey, null);
                    }
                }
                postEntry = postEntry ?? Entry.POST(OperationKey, Resource);

                yield return postEntry;
            }
        }
    }
}