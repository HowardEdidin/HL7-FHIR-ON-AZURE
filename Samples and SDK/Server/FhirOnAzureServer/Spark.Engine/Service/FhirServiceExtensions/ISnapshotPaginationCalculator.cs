#region Information

// Solution:  Spark
// Spark.Engine
// File:  ISnapshotPaginationCalculator.cs
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

    public interface ISnapshotPaginationCalculator
    {
        IEnumerable<IKey> GetKeysForPage(Snapshot snapshot, int? start = null);
        int GetIndexForLastPage(Snapshot snapshot);
        int? GetIndexForNextPage(Snapshot snapshot, int? start = null);
        int? GetIndexForPreviousPage(Snapshot snapshot, int? start = null);
    }
}