#region Information

// Solution:  Spark
// Spark.Engine
// File:  ISearchService.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:53 PM

#endregion

namespace FhirOnAzure.Engine.Service.FhirServiceExtensions
{
    using Core;
    using Hl7.Fhir.Rest;

    public interface ISearchService : IFhirServiceExtension
    {
        Snapshot GetSnapshot(string type, SearchParams searchCommand);
        Snapshot GetSnapshotForEverything(IKey key);
        IKey FindSingle(string type, SearchParams searchCommand);

        IKey FindSingleOrDefault(string type, SearchParams searchCommand);
        SearchResults GetSearchResults(string type, SearchParams searchCommand);
    }
}