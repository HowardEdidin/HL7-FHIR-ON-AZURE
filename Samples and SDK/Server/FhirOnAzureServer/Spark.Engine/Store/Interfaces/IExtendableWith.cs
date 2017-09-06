#region Information

// Solution:  Spark
// Spark.Engine
// File:  IExtendableWith.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:52 PM

#endregion

namespace FhirOnAzure.Engine.Store.Interfaces
{
    public interface IExtendableWith<T>
    {
        void AddExtension<TV>(TV extension) where TV : T;
        void RemoveExtension<TV>() where TV : T;
        TV FindExtension<TV>() where TV : T;
    }

    public interface IExtension<in T>
    {
        void OnExtensionAdded(T extensibleObject);
    }
}