#region Information

// Solution:  Spark
// Spark.Engine
// File:  HistoryService.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:53 PM

#endregion

namespace FhirOnAzure.Engine.Service.FhirServiceExtensions
{
    using Core;
    using Store.Interfaces;

    public class HistoryService : IHistoryService
    {
        private readonly IHistoryStore historyStore;
/*
        private readonly IFhirStore fhirStore;
*/

        public HistoryService(IHistoryStore historyStore)
        {
            this.historyStore = historyStore;
        }

        public Snapshot History(string typename, HistoryParameters parameters)
        {
            return historyStore.History(typename, parameters);
        }

        public Snapshot History(IKey key, HistoryParameters parameters)
        {
            return historyStore.History(key, parameters);
        }

        public Snapshot History(HistoryParameters parameters)
        {
            return historyStore.History(parameters);
        }
    }
}