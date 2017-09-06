#region Information

// Solution:  Spark
// Spark.Engine
// File:  IFhirResponseInterceptor.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:03 PM

#endregion

namespace FhirOnAzure.Engine.Interfaces
{
    using Core;

    public interface IFhirResponseInterceptor
    {
        FhirResponse GetFhirResponse(Entry entry, object input);

        bool CanHandle(object input);
    }
}