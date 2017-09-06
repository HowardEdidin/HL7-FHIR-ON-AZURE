#region Information

// Solution:  Spark
// Spark.Engine
// File:  ScopedFhirServiceBuilder.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:56 PM

#endregion

namespace FhirOnAzure.Engine.Service.ServiceIntegration
{
    using System;
    using FhirOnAzure.Service;
    using Store.Interfaces;

    public class ScopedFhirServiceBuilder<TScope> : IScopedFhirServiceBuilder<TScope>
    {
        private readonly Uri baseUri;
        private readonly IFhirService fhirService;
        private readonly IServiceListener[] listeners;
        private readonly IStorageBuilder<TScope> storageBuilder;

        public ScopedFhirServiceBuilder(Uri baseUri, IStorageBuilder<TScope> storageBuilder,
            IServiceListener[] listeners = null)
        {
            this.baseUri = baseUri;
            this.storageBuilder = storageBuilder;
            this.listeners = listeners;
            fhirService = FhirServiceFactory.GetFhirService(baseUri, storageBuilder, listeners);
        }

        public IFhirService WithScope(TScope scope)
        {
            storageBuilder.ConfigureScope(scope);
            return fhirService;
        }
    }
}