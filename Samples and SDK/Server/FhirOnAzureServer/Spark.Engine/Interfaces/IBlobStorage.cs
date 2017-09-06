#region Information

// Solution:  Spark
// Spark.Engine
// File:  IBlobStorage.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:03 PM

#endregion

namespace FhirOnAzure.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public interface IBlobStorage : IDisposable
    {
        void Open();
        void Close();
        void Store(string blobName, Stream data);
        void Delete(string blobName);
        void Delete(IEnumerable<string> names);
        byte[] Fetch(string blobName);
        string[] ListNames();
        void DeleteAll();
    }
}