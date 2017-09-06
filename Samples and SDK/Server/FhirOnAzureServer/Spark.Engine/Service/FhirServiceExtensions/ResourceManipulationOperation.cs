#region Information

// Solution:  Spark
// Spark.Engine
// File:  ResourceManipulationOperation.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:53 PM

#endregion

namespace FhirOnAzure.Engine.Service.FhirServiceExtensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Core;
    using Hl7.Fhir.Model;
    using Hl7.Fhir.Rest;

    public abstract class ResourceManipulationOperation
    {
        private readonly SearchParams searchCommand;


        private IEnumerable<Entry> interactions;

        protected ResourceManipulationOperation(Resource resource, IKey operationKey, SearchResults searchResults,
            SearchParams searchCommand = null)
        {
            this.searchCommand = searchCommand;
            Resource = resource;
            OperationKey = operationKey;
            SearchResults = searchResults;
        }

        public IKey OperationKey { get; }
        public Resource Resource { get; }
        public SearchResults SearchResults { get; }

        public IEnumerable<Entry> GetEntries()
        {
            interactions = interactions ?? ComputeEntries();
            return interactions;
        }

        protected abstract IEnumerable<Entry> ComputeEntries();

        protected string GetSearchInformation()
        {
            if (SearchResults == null)
                return null;

            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine();
            if (searchCommand != null)
            {
                var parametersNotUsed =
                    searchCommand.Parameters.Where(p => SearchResults.UsedParameters.Contains(p.Item1) == false)
                        .Select(t => t.Item1).ToArray();
                messageBuilder.AppendFormat("Search parameters not used:{0}", string.Join(",", parametersNotUsed));
                messageBuilder.AppendLine();
            }

            messageBuilder.AppendFormat("Search uri used: {0}?{1}", Resource.TypeName, SearchResults.UsedParameters);
            messageBuilder.AppendLine();
            messageBuilder.AppendFormat("Number of matches found: {0}", SearchResults.MatchCount);

            return messageBuilder.ToString();
        }
    }
}