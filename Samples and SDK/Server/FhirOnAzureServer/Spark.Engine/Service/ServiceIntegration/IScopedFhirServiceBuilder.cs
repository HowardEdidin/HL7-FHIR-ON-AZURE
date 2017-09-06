#region Information

// Solution:  Spark
// Spark.Engine
// File:  IScopedFhirServiceBuilder.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:56 PM

#endregion

namespace FhirOnAzure.Engine.Service.ServiceIntegration
{
    using FhirOnAzure.Service;

    public interface IScopedFhirServiceBuilder<in T>
    {
        IFhirService WithScope(T scope);
    }
}