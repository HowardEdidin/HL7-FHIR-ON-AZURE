#region Information

// Solution:  Spark
// Spark.Engine
// File:  IFhirStoreFull.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:03 PM

#endregion

namespace FhirOnAzure.Engine.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Core;

    public interface IFhirStoreFull
    {
        void Add(Entry entry);
        Entry Get(IKey key);
        IList<Entry> Get(IEnumerable<string> identifiers, string sortby = null);

        // primary keys
        IList<string> List(string typename, DateTimeOffset? since = null);

        IList<string> History(string typename, DateTimeOffset? since = null);
        IList<string> History(IKey key, DateTimeOffset? since = null);
        IList<string> History(DateTimeOffset? since = null);

        // BundleEntries
        bool Exists(IKey key);

        IList<Entry> GetCurrent(IEnumerable<string> identifiers, string sortby = null);

        void Add(IEnumerable<Entry> entries);

        void Replace(Entry entry);
    }

    public interface IFhirStoreAdministration
    {
        void Clean();
    }
}