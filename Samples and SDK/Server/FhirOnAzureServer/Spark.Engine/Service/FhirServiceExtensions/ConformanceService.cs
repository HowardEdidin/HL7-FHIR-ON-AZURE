#region Information

// Solution:  Spark
// Spark.Engine
// File:  ConformanceService.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:53 PM

#endregion

namespace FhirOnAzure.Engine.Service.FhirServiceExtensions
{
    using Core;
    using Hl7.Fhir.Model;

    public class CapabilityStatementService : ICapabilityStatementService
    {
        private readonly ILocalhost localhost;

        public CapabilityStatementService(ILocalhost localhost)
        {
            this.localhost = localhost;
        }

        public CapabilityStatement GetSparkCapabilityStatement(string sparkVersion)
        {
            return CapabilityStatementBuilder.GetSparkCapabilityStatement(sparkVersion, localhost);
        }
    }
}