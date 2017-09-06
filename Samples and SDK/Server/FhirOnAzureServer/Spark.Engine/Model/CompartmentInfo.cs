#region Information

// Solution:  Spark
// Spark.Engine
// File:  CompartmentInfo.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:01 PM

#endregion

namespace FhirOnAzure.Engine.Model
{
    using System.Collections.Generic;
    using Hl7.Fhir.Model;

    /// <summary>
    ///     Class for holding information as present in a CompartmentDefinition resource.
    ///     This is a (hopefully) temporary solution, since the Hl7.Fhir api does not containt CompartmentDefinition yet.
    /// </summary>
    public class CompartmentInfo
    {
        public CompartmentInfo(ResourceType resourceType)
        {
            ResourceType = resourceType;
        }

        public ResourceType ResourceType { get; set; }
        public List<string> ReverseIncludes { get; } = new List<string>();

        public void AddReverseInclude(string revInclude)
        {
            ReverseIncludes.Add(revInclude);
        }

        public void AddReverseIncludes(IEnumerable<string> revIncludes)
        {
            ReverseIncludes.AddRange(revIncludes);
        }
    }
}