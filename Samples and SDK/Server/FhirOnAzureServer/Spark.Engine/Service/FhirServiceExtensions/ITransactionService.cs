#region Information

// Solution:  Spark
// Spark.Engine
// File:  ITransactionService.cs
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
    using Core;
    using Hl7.Fhir.Model;

    public interface ITransactionService : IFhirServiceExtension
    {
        FhirResponse HandleTransaction(ResourceManipulationOperation operation, IInteractionHandler interactionHandler);
        IList<Tuple<Entry, FhirResponse>> HandleTransaction(Bundle bundle, IInteractionHandler interactionHandler);

        IList<Tuple<Entry, FhirResponse>> HandleTransaction(IList<Entry> interactions,
            IInteractionHandler interactionHandler);
    }
}