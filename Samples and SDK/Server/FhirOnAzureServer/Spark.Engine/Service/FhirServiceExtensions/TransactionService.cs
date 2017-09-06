#region Information

// Solution:  Spark
// Spark.Engine
// File:  TransactionService.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:53 PM

#endregion

namespace FhirOnAzure.Engine.Service.FhirServiceExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using FhirOnAzure.Service;
    using Hl7.Fhir.Model;

    public class TransactionService : ITransactionService
    {
        private readonly ILocalhost localhost;
        private readonly ISearchService searchService;
        private readonly ITransfer transfer;

        public TransactionService(ILocalhost localhost, ITransfer transfer, ISearchService searchService)
        {
            this.localhost = localhost;
            this.transfer = transfer;
            this.searchService = searchService;
        }

        public IList<Tuple<Entry, FhirResponse>> HandleTransaction(IList<Entry> interactions,
            IInteractionHandler interactionHandler)
        {
            if (interactionHandler == null)
                throw new InvalidOperationException("Unable to run transaction operation");

            return HandleTransaction(interactions, interactionHandler, null);
        }

        public FhirResponse HandleTransaction(ResourceManipulationOperation operation,
            IInteractionHandler interactionHandler)
        {
            return HandleOperation(operation, interactionHandler);
        }

        public IList<Tuple<Entry, FhirResponse>> HandleTransaction(Bundle bundle,
            IInteractionHandler interactionHandler)
        {
            if (interactionHandler == null)
                throw new InvalidOperationException("Unable to run transaction operation");

            var entries = new List<Entry>();
            var mapper = new Mapper<string, IKey>();

            foreach (var operation in bundle.Entry.Select(
                e => ResourceManipulationOperationFactory.GetManipulationOperation(e, localhost, searchService)))
            {
                IList<Entry> atomicOperations = operation.GetEntries().ToList();
                AddMappingsForOperation(mapper, operation, atomicOperations);
                entries.AddRange(atomicOperations);
            }

            return HandleTransaction(entries, interactionHandler, mapper);
        }

        public FhirResponse HandleOperation(ResourceManipulationOperation operation,
            IInteractionHandler interactionHandler, Mapper<string, IKey> mapper = null)
        {
            IList<Entry> interactions = operation.GetEntries().ToList();
            if (mapper != null)
                transfer.Internalize(interactions, mapper);

            FhirResponse response = null;
            foreach (var interaction in interactions)
            {
                response = MergeFhirResponse(response, interactionHandler.HandleInteraction(interaction));
                if (!response.IsValid) throw new Exception();
                interaction.Resource = response.Resource;
            }

            transfer.Externalize(interactions);

            return response;
        }

        private FhirResponse MergeFhirResponse(FhirResponse previousResponse, FhirResponse response)
        {
            //CCR: How to handle responses?
            //Currently we assume that all FhirResponses from one ResourceManipulationOperation should be equivalent - kind of hackish
            if (previousResponse == null)
                return response;
            if (!response.IsValid)
                return response;
            if (response.StatusCode != previousResponse.StatusCode)
                throw new Exception("Incompatible responses");
            if (response.Key != null && previousResponse.Key != null &&
                response.Key.Equals(previousResponse.Key) == false)
                throw new Exception("Incompatible responses");
            if (response.Key != null && previousResponse.Key == null ||
                response.Key == null && previousResponse.Key != null)
                throw new Exception("Incompatible responses");
            return response;
        }

        private void AddMappingsForOperation(Mapper<string, IKey> mapper, ResourceManipulationOperation operation,
            IList<Entry> interactions)
        {
            if (mapper == null)
                return;
            if (interactions.Count() == 1)
            {
                var entry = interactions.First();
                if (!entry.Key.Equals(operation.OperationKey))
                    if (localhost.GetKeyKind(operation.OperationKey) == KeyKind.Temporary)
                        mapper.Remap(operation.OperationKey.ResourceId, entry.Key.WithoutVersion());
                    else
                        mapper.Remap(operation.OperationKey.ToString(), entry.Key.WithoutVersion());
            }
        }

        private IList<Tuple<Entry, FhirResponse>> HandleTransaction(IList<Entry> interactions,
            IInteractionHandler interactionHandler, Mapper<string, IKey> mapper)
        {
            var responses = new List<Tuple<Entry, FhirResponse>>();

            transfer.Internalize(interactions, mapper);

            foreach (var interaction in interactions)
            {
                var response = interactionHandler.HandleInteraction(interaction);
                if (!response.IsValid) throw new Exception();
                interaction.Resource = response.Resource;
                response.Resource = null;

                responses.Add(
                    new Tuple<Entry, FhirResponse>(interaction,
                        response)); //CCR: How to handle responses for transactions? 
                //The specifications says only one response should be sent per EntryComponent, 
                //but one EntryComponent might correpond to multiple atomic entries (Entry)
                //Example: conditional delete
            }

            transfer.Externalize(interactions);
            return responses;
        }
    }
}