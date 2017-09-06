#region Information

// Solution:  Spark
// Spark.Engine
// File:  PagingService.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:53 PM

#endregion

namespace FhirOnAzure.Engine.Service.FhirServiceExtensions
{
    using System;
    using Core;
    using Store.Interfaces;

    public class PagingService : IPagingService
    {
        private readonly ISnapshotPaginationProvider _paginationProvider;
        private readonly ISnapshotStore _snapshotstore;

        public PagingService(ISnapshotStore snapshotstore, ISnapshotPaginationProvider paginationProvider)
        {
            _snapshotstore = snapshotstore;
            _paginationProvider = paginationProvider;
        }

        public ISnapshotPagination StartPagination(Snapshot snapshot)
        {
            if (_snapshotstore != null)
                _snapshotstore.AddSnapshot(snapshot);
            else
                snapshot.Id = null;

            return _paginationProvider.StartPagination(snapshot);
        }

        public ISnapshotPagination StartPagination(string snapshotkey)
        {
            if (_snapshotstore == null)
                throw new NotSupportedException("Stateful pagination is not currently supported.");
            return _paginationProvider.StartPagination(_snapshotstore.GetSnapshot(snapshotkey));
        }
    }
}