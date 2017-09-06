#region Information

// Solution:  Spark
// Spark.Engine
// File:  FhirServiceFactory.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:56 PM

#endregion

namespace FhirOnAzure.Engine.Service.ServiceIntegration
{
    using System;
    using System.Linq;
    using Core;
    using FhirOnAzure.Core;
    using FhirOnAzure.Service;
    using FhirResponseFactory;
    using FhirServiceExtensions;
    using Interfaces;
    using Store.Interfaces;

    public class FhirServiceFactory
    {
        public static IFhirService GetFhirService(Uri baseUri, IStorageBuilder storageBuilder,
            IServiceListener[] listeners = null)
        {
            var extensionsBuilder = new FhirExtensionsBuilder(storageBuilder, baseUri);
            return GetFhirService(baseUri, extensionsBuilder, storageBuilder, listeners);
        }

        public static IFhirService GetFhirService(Uri baseUri, IFhirExtensionsBuilder extensionsBuilder,
            IStorageBuilder storageBuilder, //we won't need this anymore if we can remove the Transfer dependency for IFhirService
            IServiceListener[] listeners = null)
        {
            var generator = storageBuilder.GetStore<IGenerator>();
            var extensions = extensionsBuilder.GetExtensions().ToArray();
            var computedListeners = (listeners ?? Enumerable.Empty<IServiceListener>())
                .Union(extensions.OfType<IServiceListener>())
                .ToArray();
            ICompositeServiceListener serviceListener = new ServiceListener(new Localhost(baseUri), computedListeners);
            var transfer = new Transfer(generator, new Localhost(baseUri));

            return new FhirService(extensions,
                GetFhirResponseFactory(baseUri),
                transfer,
                serviceListener);
        }


        private static IFhirResponseFactory GetFhirResponseFactory(Uri baseUri)
        {
            return new FhirResponseFactory(new Localhost(baseUri),
                new FhirResponseInterceptorRunner(new IFhirResponseInterceptor[] {new ConditionalHeaderFhirResponseInterceptor()}));
        }
    }
}