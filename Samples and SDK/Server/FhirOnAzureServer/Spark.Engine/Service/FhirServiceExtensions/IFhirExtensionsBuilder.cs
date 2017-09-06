#region Information

// Solution:  Spark
// Spark.Engine
// File:  IFhirExtensionsBuilder.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:53 PM

#endregion

namespace FhirOnAzure.Engine.Service.FhirServiceExtensions
{
    using System.Collections.Generic;

    public interface IFhirExtensionsBuilder : IEnumerable<IFhirServiceExtension>
    {
        IEnumerable<IFhirServiceExtension> GetExtensions();
    }
}