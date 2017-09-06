#region Information

// Solution:  Spark
// Spark.Engine
// File:  IResourceValidator.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:03 PM

#endregion

namespace FhirOnAzure.Core.Interfaces
{
    using System.Collections.Generic;
    using Hl7.Fhir.Model;

    internal interface IResourceValidator
    {
        IEnumerable<OperationOutcome> Validate(Resource resource);
    }
}