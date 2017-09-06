using Hl7.Fhir.Model;
using Microsoft.Practices.Unity;
using FhirOnAzure.Controllers;
using FhirOnAzure.Core;
using FhirOnAzure.Engine.Core;
using FhirOnAzure.Models;
using FhirOnAzure.Mongo.Search.Common;
using FhirOnAzure.Service;
using FhirOnAzure.Store.Mongo;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using FhirOnAzure.Engine.FhirResponseFactory;
using FhirOnAzure.Engine.Interfaces;
using Unity.WebApi;
using FhirOnAzure.Mongo.Search.Indexer;
using FhirOnAzure.Import;
using FhirOnAzure.Engine.Model;
using FhirOnAzure.Engine.Service;
using FhirOnAzure.Engine.Service.FhirServiceExtensions;
using FhirOnAzure.Engine.Store.Interfaces;
using FhirOnAzure.Filters;
using FhirOnAzure.Mongo.Store;
using FhirOnAzure.Mongo.Store.Extensions;

namespace FhirOnAzure
{
    public static class UnityConfig
    {
        public static void RegisterComponents(HttpConfiguration config)
        {
            var container = GetUnityContainer();

            // e.g. container.RegisterType<ITestService, TestService>();
            IControllerFactory unityControllerFactory = new UnityControllerFactory(container);
            ControllerBuilder.Current.SetControllerFactory(unityControllerFactory);

            config.DependencyResolver = new UnityDependencyResolver(container);
            GlobalHost.DependencyResolver = new SignalRUnityDependencyResolver(container);
        }

        public static UnityContainer GetUnityContainer()
        {
            var container = new UnityContainer();

#if DEBUG
            container.AddNewExtension<UnityLogExtension>();
#endif
            container.RegisterType<HomeController, HomeController>(new PerResolveLifetimeManager(),
                new InjectionConstructor(Settings.MongoUrl));
            container.RegisterType<IServiceListener, ServiceListener>(new ContainerControlledLifetimeManager());
            container.RegisterType<ILocalhost, Localhost>(new ContainerControlledLifetimeManager(),
                new InjectionConstructor(Settings.Endpoint));
            //container.RegisterType<MongoFhirStore, MongoFhirStore>(new ContainerControlledLifetimeManager(),
            //    new InjectionConstructor(Settings.MongoUrl));
            container.RegisterType<IFhirStore, MongoFhirStore>(new ContainerControlledLifetimeManager(),
                   new InjectionConstructor(Settings.MongoUrl));
            container.RegisterType<IGenerator, MongoIdGenerator>(new ContainerControlledLifetimeManager(),
                new InjectionConstructor(Settings.MongoUrl));
            container.RegisterType<ISnapshotStore, MongoSnapshotStore>(new ContainerControlledLifetimeManager(),
                  new InjectionConstructor(Settings.MongoUrl));
            container.RegisterType<IFhirStoreAdministration, MongoStoreAdministration>(new ContainerControlledLifetimeManager(),
                     new InjectionConstructor(Settings.MongoUrl));
            container.RegisterType<IIndexStore, MongoIndexStore>(new ContainerControlledLifetimeManager());

            container.RegisterType<ITransfer, Transfer>(new ContainerControlledLifetimeManager());
            container.RegisterType<MongoIndexStore>(new ContainerControlledLifetimeManager(),
                new InjectionConstructor(Settings.MongoUrl, container.Resolve<MongoIndexMapper>()));
            container.RegisterInstance<Definitions>(DefinitionsFactory.Generate(ModelInfo.SearchParameters));
            //TODO: Use FhirModel instead of ModelInfo
            container.RegisterType<IFhirIndex, MongoFhirIndex>(new ContainerControlledLifetimeManager());
            container.RegisterType<IFhirResponseFactoryOld, FhirResponseFactoryOld>();
            container.RegisterType<IFhirResponseFactory, FhirResponseFactory>();
            container.RegisterType<IFhirResponseInterceptorRunner, FhirResponseInterceptorRunner>();
            container.RegisterType<IFhirResponseInterceptor, ConditionalHeaderFhirResponseInterceptor>("ConditionalHeaderFhirResponseInterceptor");
            container.RegisterType<IFhirModel, FhirModel>(new ContainerControlledLifetimeManager(),
                new InjectionConstructor(SparkModelInfo.SparkSearchParameters));
            container.RegisterType<FhirPropertyIndex>(new ContainerControlledLifetimeManager(),
                new InjectionConstructor(container.Resolve<IFhirModel>()));

            container.RegisterType<CompressionHandler>(new ContainerControlledLifetimeManager(),
                new InjectionConstructor(Settings.MaximumDecompressedBodySizeInBytes));

            container.RegisterType<InitializerHub>(new HierarchicalLifetimeManager());
            container.RegisterType<IHistoryStore, HistoryStore>(new InjectionConstructor(Settings.MongoUrl));
            container.RegisterType<IFhirService, FhirService>(new ContainerControlledLifetimeManager());
            //new InjectionFactory(unityContainer => unityContainer.Resolve<IFhirService>
            //(new DependencyOverride(typeof(IFhirServiceExtension[]), 
            //unityContainer.Resolve<IFhirExtensionsBuilder>().GetExtensions()))));

            container.RegisterType<IServiceListener, SearchService>("searchListener");
            container.RegisterType<IFhirServiceExtension, SearchService>("search");
            container.RegisterType<ISearchService, SearchService>();
            container.RegisterType<IFhirServiceExtension, TransactionService>("transaction");
            container.RegisterType<IFhirServiceExtension, HistoryService>("history");
            container.RegisterType<IFhirServiceExtension, PagingService>("paging");
            container.RegisterType<ISnapshotPaginationProvider, SnapshotPaginationProvider>();
            container.RegisterType<ISnapshotPaginationCalculator, SnapshotPaginationCalculator>();
            container.RegisterType<IFhirServiceExtension, ResourceStorageService>("storage");
            container.RegisterType<IFhirServiceExtension, CapabilityStatementService>("capabilityStatement");
            container.RegisterType<ICompositeServiceListener, ServiceListener>(new ContainerControlledLifetimeManager());

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            return container;
        }
    }
}

//using Microsoft.Practices.Unity;
//using System.Web.Http;
//using Unity.WebApi;

//namespace FhirOnAzure
//{
//    public static class UnityConfig
//    {
//        public static void RegisterComponents()
//        {
//			var container = new UnityContainer();

//            // register all your components with the container here
//            // it is NOT necessary to register your controllers

//            // e.g. container.RegisterType<ITestService, TestService>();

//            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
//        }
//    }
//}