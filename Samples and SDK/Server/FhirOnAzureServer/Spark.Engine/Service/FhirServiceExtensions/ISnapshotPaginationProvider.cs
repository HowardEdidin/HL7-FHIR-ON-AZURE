#region Information

// Solution:  Spark
// Spark.Engine
// File:  ISnapshotPaginationProvider.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:53 PM

#endregion

namespace FhirOnAzure.Engine.Service.FhirServiceExtensions
{
    using Core;

    public interface ISnapshotPaginationProvider
    {
        ISnapshotPagination StartPagination(Snapshot snapshot);
    }
}