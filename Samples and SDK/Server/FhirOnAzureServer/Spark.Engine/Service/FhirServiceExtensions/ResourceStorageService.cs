#region Information

// Solution:  Spark
// Spark.Engine
// File:  ResourceStorageService.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:53 PM

#endregion

namespace FhirOnAzure.Engine.Service.FhirServiceExtensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using FhirOnAzure.Service;
    using Store.Interfaces;

    public class ResourceStorageService : IResourceStorageService
    {
        private readonly ITransfer transfer;
        private readonly IFhirStore fhirStore;


        public ResourceStorageService(ITransfer transfer, IFhirStore fhirStore)
        {
            this.transfer = transfer;
            this.fhirStore = fhirStore;
        }

        public Entry Get(IKey key)
        {
            var entry = fhirStore.Get(key);
            if (entry != null)
                transfer.Externalize(entry);
            return entry;
        }

        public Entry Add(Entry entry)
        {
            if (entry.State != EntryState.Internal)
                transfer.Internalize(entry);
            fhirStore.Add(entry);
            Entry result;
            if (entry.IsDelete)
                result = entry;
            else
                result = fhirStore.Get(entry.Key);
            transfer.Externalize(result);

            return result;
        }

        public IList<Entry> Get(IEnumerable<string> localIdentifiers, string sortby = null)
        {
            var results = fhirStore.Get(localIdentifiers.Select(k => (IKey) Key.ParseOperationPath(k)));
            transfer.Externalize(results);
            return results;
        }
    }
}