#region Information

// Solution:  Spark
// Spark.Engine
// File:  SparkModelInfo.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:01 PM

#endregion

using static Hl7.Fhir.Model.ModelInfo;

namespace FhirOnAzure.Engine.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Hl7.Fhir.Model;

    public static class SparkModelInfo
    {
        public static List<SearchParamDefinition> SparkSearchParameters = SearchParameters.Union(
            new List<SearchParamDefinition>
            {
                new SearchParamDefinition
                {
                    Resource = "Composition",
                    Name = "custodian",
                    Description = @"custom search parameter on Composition for generating $document",
                    Type = SearchParamType.Reference,
                    Path = new[] {"Composition.custodian"},
                    XPath = "f:Composition/f:custodian",
                    Expression = "Composition.custodian",
                    Target = new[] {ResourceType.Organization}
                },
                new SearchParamDefinition
                {
                    Resource = "Composition",
                    Name = "eventdetail",
                    Description = @"custom search parameter on Composition for generating $document",
                    Type = SearchParamType.Reference,
                    Path = new[] {"Composition.event.detail"},
                    XPath = "f:Composition/f:event/f:detail",
                    Expression = "Composition.event.detail",
                    Target = Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>().ToArray()
                },
                new SearchParamDefinition
                {
                    Resource = "Encounter",
                    Name = "serviceprovider",
                    Description = @"Organization that provides the Encounter services.",
                    Type = SearchParamType.Reference,
                    Path = new[] {"Encounter.serviceProvider"},
                    XPath = "f:Encounter/f:serviceProvider",
                    Expression = "Encounter.serviceProvider",
                    Target = new[] {ResourceType.Organization}
                },
                new SearchParamDefinition
                {
                    Resource = "Slot",
                    Name = "provider",
                    Description = @"Search Slot by provider extension",
                    Type = SearchParamType.Reference,
                    Path = new[] {@"Slot.extension[url=http://fhir.blackpear.com/era/Slot/provider].valueReference"},
                    Target = new[] {ResourceType.Organization}
                }
            }).ToList();
    }
}