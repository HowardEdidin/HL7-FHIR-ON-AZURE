#region Information

// Solution:  Spark
// Spark.Engine
// File:  IIndexStore.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:52 PM

#endregion

namespace FhirOnAzure.Engine.Store.Interfaces
{
    using Core;
    using Model;

    public interface IIndexStore
    {
        void Save(IndexValue indexValue);

        void Delete(Entry entry);

        void Clean();
    }
}