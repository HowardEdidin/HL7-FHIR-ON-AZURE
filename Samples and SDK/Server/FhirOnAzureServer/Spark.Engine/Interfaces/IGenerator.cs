#region Information

// Solution:  Spark
// Spark.Engine
// File:  IGenerator.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:03 PM

#endregion

namespace FhirOnAzure.Core
{
    using Hl7.Fhir.Model;

    public interface IGenerator
    {
        string NextResourceId(Resource resource);
        string NextVersionId(string resourceIdentifier);
        string NextVersionId(string resourceType, string resourceIdentifier);
    }
}