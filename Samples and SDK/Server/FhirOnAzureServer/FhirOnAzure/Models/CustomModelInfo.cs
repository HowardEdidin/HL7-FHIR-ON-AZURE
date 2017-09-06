#region Information

// Solution:  Spark
// FhirOnAzure
// File:  CustomModelInfo.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:44 PM

#endregion

using static Hl7.Fhir.Model.ModelInfo;

namespace FhirOnAzure.Models
{
    using System.Collections.Generic;
    using Hl7.Fhir.Model;

    public class CustomModelInfo

    {
        static CustomModelInfo()
        {
            SearchParameters = new List<SearchParamDefinition>
            {
                new SearchParamDefinition
                {
                    Resource = "Practitioner",
                    Name = "roleid",
                    Description = @"Search by role identifier extension",
                    Type = SearchParamType.Token,
                    Path = new[]
                    {
                        @"Practitioner.practitionerRole.Extension[url=http://hl7.no/fhir/StructureDefinition/practitonerRole-identifier].ValueIdentifier"
                    }
                }
            };
//            searchParameters.AddRange(ModelInfo.SearchParameters);
        }

        public static List<SearchParamDefinition> SearchParameters { get; }
    }
}