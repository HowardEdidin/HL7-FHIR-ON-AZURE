#region Information

// Solution:  Spark
// Spark.Engine
// File:  IFhirStoreExtension.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:52 PM

#endregion

namespace FhirOnAzure.Engine.Store.Interfaces
{
    using Core;

    public interface IFhirStoreExtension : IExtension<IFhirStore>
    {
        void OnEntryAdded(Entry entry);
    }
}