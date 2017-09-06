#region Information

// Solution:  Spark
// Spark.Engine
// File:  IHistoryService.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:53 PM

#endregion

namespace FhirOnAzure.Engine.Service.FhirServiceExtensions
{
    using Core;

    internal interface IHistoryService : IFhirServiceExtension
    {
        Snapshot History(string typename, HistoryParameters parameters);
        Snapshot History(IKey key, HistoryParameters parameters);
        Snapshot History(HistoryParameters parameters);
    }
}