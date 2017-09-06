#region Information

// Solution:  Spark
// Spark.Engine
// File:  SearchParameter.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:01 PM

#endregion

namespace FhirOnAzure.Engine.Search.Model
{
    using Hl7.Fhir.Model;

    public class RichSearchParameter : SearchParameter
    {
        public readonly SearchParameter searchParameter;

        public RichSearchParameter(SearchParameter searchParameter)
        {
            this.searchParameter = searchParameter;
        }
    }
}