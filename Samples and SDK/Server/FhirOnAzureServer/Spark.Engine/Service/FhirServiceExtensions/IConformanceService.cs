#region Information

// Solution:  Spark
// Spark.Engine
// File:  IConformanceService.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:53 PM

#endregion

namespace FhirOnAzure.Engine.Service.FhirServiceExtensions
{
    using Hl7.Fhir.Model;

    internal interface ICapabilityStatementService : IFhirServiceExtension
    {
        CapabilityStatement GetSparkCapabilityStatement(string sparkVersion);
    }
}