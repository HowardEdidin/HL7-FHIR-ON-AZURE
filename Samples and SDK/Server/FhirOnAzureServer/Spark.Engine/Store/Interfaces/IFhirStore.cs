#region Information

// Solution:  Spark
// Spark.Engine
// File:  IFhirStore.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:52 PM

#endregion

namespace FhirOnAzure.Engine.Store.Interfaces
{
    using System.Collections.Generic;
    using Core;

    public interface IFhirStore
    {
        void Add(Entry entry);
        Entry Get(IKey key);
        IList<Entry> Get(IEnumerable<IKey> localIdentifiers);
    }
}