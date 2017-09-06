#region Information

// Solution:  Spark
// Spark.Engine
// File:  IFhirIndex.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:03 PM

#endregion

namespace FhirOnAzure.Core
{
    using System.Collections.Generic;
    using Engine.Core;
    using Hl7.Fhir.Rest;

    public interface IFhirIndex
    {
        void Clean();
        void Process(IEnumerable<Entry> entries);
        void Process(Entry entry);
        SearchResults Search(string resource, SearchParams searchCommand);
        Key FindSingle(string resource, SearchParams searchCommand);
        SearchResults GetReverseIncludes(IList<IKey> keys, IList<string> revIncludes);
    }
}