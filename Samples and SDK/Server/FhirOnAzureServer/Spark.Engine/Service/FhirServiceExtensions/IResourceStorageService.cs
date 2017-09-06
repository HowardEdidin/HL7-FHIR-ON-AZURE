#region Information

// Solution:  Spark
// Spark.Engine
// File:  IResourceStorageService.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:53 PM

#endregion

namespace FhirOnAzure.Engine.Service.FhirServiceExtensions
{
    using System.Collections.Generic;
    using Core;

    public interface IResourceStorageService : IFhirServiceExtension
    {
        Entry Get(IKey key);
        Entry Add(Entry entry);
        IList<Entry> Get(IEnumerable<string> localIdentifiers, string sortby = null);
    }
}