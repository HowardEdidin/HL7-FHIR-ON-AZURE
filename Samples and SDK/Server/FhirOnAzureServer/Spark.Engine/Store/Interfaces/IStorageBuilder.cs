#region Information

// Solution:  Spark
// Spark.Engine
// File:  IStorageBuilder.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:52 PM

#endregion

namespace FhirOnAzure.Engine.Store.Interfaces
{
    public interface IStorageBuilder
    {
        T GetStore<T>();
    }


    public interface IStorageBuilder<in TScope> : IStorageBuilder
    {
        void ConfigureScope(TScope scope);
    }
}