#region Information

// Solution:  Spark
// Spark.Engine
// File:  ConformanceBuilder.cs
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
    using Hl7.Fhir.Model;
    using Hl7.Fhir.Serialization;
    using Hl7.Fhir.Utility;

    public static class CapabilityStatementBuilder
    {
        public static CapabilityStatement GetSparkCapabilityStatement(string sparkVersion, ILocalhost localhost)
        {
            var vsn = ModelInfo.Version;
            var capabilityStatement = CreateServer("FhironAzure", sparkVersion, "Microsoft", vsn);

            capabilityStatement.AddAllCoreResources(true, true,
                CapabilityStatement.ResourceVersionPolicy.VersionedUpdate);
            capabilityStatement.AddAllSystemInteractions().AddAllInteractionsForAllResources()
                .AddCoreSearchParamsAllResources();
            capabilityStatement.AddSummaryForAllResources();
            capabilityStatement.AddOperation("Fetch Patient Record",
                new ResourceReference
                {
                    Url = localhost.Absolute(new Uri("OperationDefinition/Patient-everything", UriKind.Relative))
                });
            capabilityStatement.AddOperation("Generate a Document",
                new ResourceReference
                {
                    Url = localhost.Absolute(new Uri("OperationDefinition/Composition-document", UriKind.Relative))
                });
            capabilityStatement.AcceptUnknown = CapabilityStatement.UnknownContentCode.Both;
            capabilityStatement.Experimental = true;
            capabilityStatement.Kind = CapabilityStatement.CapabilityStatementKind.Capability;
            capabilityStatement.Format = new[] {"xml", "json"};
            capabilityStatement.Description =
                new Markdown(
                    "This FHIR SERVER is a reference Implementation server built in C# on HL7.Fhir.Core (nuget) and deployed on Microsoft Azure");

            return capabilityStatement;
        }

        public static CapabilityStatement CreateServer(string server, string serverVersion, string publisher,
            string fhirVersion)
        {
            var capabilityStatement = new CapabilityStatement
            {
                Name = server,
                Publisher = publisher,
                Version = serverVersion,
                FhirVersion = fhirVersion,
                AcceptUnknown = CapabilityStatement.UnknownContentCode.No,
                Date = Date.Today().Value
            };
            capabilityStatement.AddServer();
            return capabilityStatement;
            //AddRestComponent(true);
            //AddAllCoreResources(true, true, CapabilityStatement.ResourceVersionPolicy.VersionedUpdate);
            //AddAllSystemInteractions();
            //AddAllResourceInteractionsAllResources();
            //AddCoreSearchParamsAllResources();

            //return con;
        }

        public static CapabilityStatement.RestComponent AddRestComponent(this CapabilityStatement capabilityStatement,
            bool isServer, string documentation = null)
        {
            var server = new CapabilityStatement.RestComponent
            {
                Mode = isServer
                ? CapabilityStatement.RestfulCapabilityMode.Server
                : CapabilityStatement.RestfulCapabilityMode.Client
            };

            if (documentation != null)
                server.Documentation = documentation;
            capabilityStatement.Rest.Add(server);
            return server;
        }

        public static CapabilityStatement AddServer(this CapabilityStatement capabilityStatement)
        {
            capabilityStatement.AddRestComponent(true);
            return capabilityStatement;
        }

        public static CapabilityStatement.RestComponent Server(this CapabilityStatement capabilityStatement)
        {
            var server =
                capabilityStatement.Rest.FirstOrDefault(
                    r => r.Mode == CapabilityStatement.RestfulCapabilityMode.Server);
            return server ?? capabilityStatement.AddRestComponent(true);
        }

        public static CapabilityStatement.RestComponent Rest(this CapabilityStatement capabilityStatement)
        {
            return capabilityStatement.Rest.FirstOrDefault();
        }

        public static CapabilityStatement AddAllCoreResources(this CapabilityStatement capabilityStatement,
            bool readhistory, bool updatecreate, CapabilityStatement.ResourceVersionPolicy versioning)
        {
            foreach (var resource in ModelInfo.SupportedResources)
                capabilityStatement.AddSingleResourceComponent(resource, readhistory, updatecreate, versioning);
            return capabilityStatement;
        }

        public static CapabilityStatement AddMultipleResourceComponents(this CapabilityStatement capabilityStatement,
            List<string> resourcetypes, bool readhistory, bool updatecreate,
            CapabilityStatement.ResourceVersionPolicy versioning)
        {
            foreach (var type in resourcetypes)
                AddSingleResourceComponent(capabilityStatement, type, readhistory, updatecreate, versioning);
            return capabilityStatement;
        }

        public static CapabilityStatement AddSingleResourceComponent(this CapabilityStatement capabilityStatement,
            string resourcetype, bool readhistory, bool updatecreate,
            CapabilityStatement.ResourceVersionPolicy versioning, ResourceReference profile = null)
        {
            var resource = new CapabilityStatement.ResourceComponent
            {
                Type = Hacky.GetResourceTypeForResourceName(resourcetype),
                Profile = profile,
                ReadHistory = readhistory,
                UpdateCreate = updatecreate,
                Versioning = versioning
            };
            capabilityStatement.Server().Resource.Add(resource);
            return capabilityStatement;
        }

        public static CapabilityStatement AddSummaryForAllResources(this CapabilityStatement capabilityStatement)
        {
            foreach (var resource in capabilityStatement.Rest.FirstOrDefault().Resource.ToList())
            {
                var p = new CapabilityStatement.SearchParamComponent
                {
                    Name = "_summary",
                    Type = SearchParamType.String,
                    Documentation = "Summary for resource"
                };
                resource.SearchParam.Add(p);
            }
            return capabilityStatement;
        }

        public static CapabilityStatement AddCoreSearchParamsAllResources(this CapabilityStatement capabilityStatement)
        {
            foreach (var r in capabilityStatement.Rest.FirstOrDefault().Resource.ToList())
            {
                capabilityStatement.Rest().Resource.Remove(r);
                capabilityStatement.Rest().Resource.Add(AddCoreSearchParamsResource(r));
            }
            return capabilityStatement;
        }


        public static CapabilityStatement.ResourceComponent AddCoreSearchParamsResource(
            CapabilityStatement.ResourceComponent resourcecomp)
        {
            var parameters = ModelInfo.SearchParameters.Where(sp => sp.Resource == resourcecomp.Type.GetLiteral())
                .Select(sp => new CapabilityStatement.SearchParamComponent
                {
                    Name = sp.Name,
                    Type = sp.Type,
                    Documentation = sp.Description
                });

            resourcecomp.SearchParam.AddRange(parameters);
            return resourcecomp;
        }

        public static CapabilityStatement AddAllInteractionsForAllResources(
            this CapabilityStatement capabilityStatement)
        {
            foreach (var r in capabilityStatement.Rest.FirstOrDefault().Resource.ToList())
            {
                capabilityStatement.Rest().Resource.Remove(r);
                capabilityStatement.Rest().Resource.Add(AddAllResourceInteractions(r));
            }
            return capabilityStatement;
        }

        public static CapabilityStatement.ResourceComponent AddAllResourceInteractions(
            CapabilityStatement.ResourceComponent resourcecomp)
        {
            foreach (CapabilityStatement.TypeRestfulInteraction type in Enum.GetValues(
                typeof(CapabilityStatement.TypeRestfulInteraction)))
            {
                var interaction = AddSingleResourceInteraction(resourcecomp, type);
                resourcecomp.Interaction.Add(interaction);
            }
            return resourcecomp;
        }

        public static CapabilityStatement.ResourceInteractionComponent AddSingleResourceInteraction(
            CapabilityStatement.ResourceComponent resourcecomp, CapabilityStatement.TypeRestfulInteraction type)
        {
            var interaction = new CapabilityStatement.ResourceInteractionComponent
            {
                Code = type
            };
            return interaction;
        }

        public static CapabilityStatement AddAllSystemInteractions(this CapabilityStatement capabilityStatement)
        {
            foreach (CapabilityStatement.SystemRestfulInteraction code in Enum.GetValues(
                typeof(CapabilityStatement.SystemRestfulInteraction)))
                capabilityStatement.AddSystemInteraction(code);
            return capabilityStatement;
        }

        public static void AddSystemInteraction(this CapabilityStatement capabilityStatement,
            CapabilityStatement.SystemRestfulInteraction code)
        {
            var interaction = new CapabilityStatement.SystemInteractionComponent
            {
                Code = code
            };

            capabilityStatement.Rest().Interaction.Add(interaction);
        }

        public static void AddOperation(this CapabilityStatement capabilityStatement, string name,
            ResourceReference definition)
        {
            var operation = new CapabilityStatement.OperationComponent
            {
                Name = name,
                Definition = definition
            };

            capabilityStatement.Server().Operation.Add(operation);
        }

        public static string CapabilityStatementToXML(this CapabilityStatement capabilityStatement)
        {
            return FhirSerializer.SerializeResourceToXml(capabilityStatement);
        }
    }
}

// TODO: Code review CapabilityStatement replacement
//public const string CONFORMANCE_ID = "self";
//public static readonly string CONFORMANCE_COLLECTION_NAME = typeof(CapabilityStatement).GetCollectionName();

//public static CapabilityStatement CreateTemp()
//{
//    return new CapabilityStatement();

//}

//public static CapabilityStatement Build()
//{
//    //var capabilityStatement = new CapabilityStatement();

//Stream s = typeof(CapabilityStatementBuilder).Assembly.GetManifestResourceStream("FhirOnAzure.Engine.Service.CapabilityStatement.xml");
//StreamReader sr = new StreamReader(s);
//string capabilityStatementXml = sr.ReadToEnd();

//var capabilityStatement = (CapabilityStatement)FhirParser.ParseResourceFromXml(capabilityStatementXml);

//capabilityStatement.Software = new CapabilityStatement.CapabilityStatementSoftwareComponent();
//capabilityStatement.Software.Version = ReadVersionFromAssembly();
//capabilityStatement.Software.Name = ReadProductNameFromAssembly();
//capabilityStatement.FhirVersion = ModelInfo.Version;
//capabilityStatement.Date = Date.Today().Value;
//capabilityStatement.Meta = new Resource.ResourceMetaComponent();
//capabilityStatement.Meta.VersionId = "0";

//CapabilityStatement.CapabilityStatementRestComponent serverComponent = capabilityStatement.Rest[0];

// Replace anything that was there before...
//serverComponent.Resource = new List<CapabilityStatement.CapabilityStatementRestResourceComponent>();

/*var allOperations = new List<CapabilityStatement.CapabilityStatementRestResourceOperationComponent>()
{   new CapabilityStatement.CapabilityStatementRestResourceOperationComponent { Code = CapabilityStatement.RestfulOperationType.Create },
    new CapabilityStatement.CapabilityStatementRestResourceOperationComponent { Code = CapabilityStatement.RestfulOperationType.Delete },
    new CapabilityStatement.CapabilityStatementRestResourceOperationComponent { Code = CapabilityStatement.RestfulOperationType.HistoryInstance },

    new CapabilityStatement.CapabilityStatementRestResourceOperationComponent { Code = CapabilityStatement.RestfulOperationType.HistoryType },
    new CapabilityStatement.CapabilityStatementRestResourceOperationComponent { Code = CapabilityStatement.RestfulOperationType.Read },

    new CapabilityStatement.CapabilityStatementRestResourceOperationComponent { Code = CapabilityStatement.RestfulOperationType.SearchType },


    new CapabilityStatement.CapabilityStatementRestResourceOperationComponent { Code = CapabilityStatement.RestfulOperationType.Update },
    new CapabilityStatement.CapabilityStatementRestResourceOperationComponent { Code = CapabilityStatement.RestfulOperationType.Validate },
    new CapabilityStatement.CapabilityStatementRestResourceOperationComponent { Code = CapabilityStatement.RestfulOperationType.Vread },
};

foreach (var resourceName in ModelInfo.SupportedResources)
{
    var supportedResource = new CapabilityStatement.CapabilityStatementRestResourceComponent();
    supportedResource.Type = resourceName;
    supportedResource.ReadHistory = true;
    supportedResource.Operation = allOperations;

    // Add supported _includes for this resource
    supportedResource.SearchInclude = ModelInfo.SearchParameters
        .Where(sp => sp.Resource == resourceName)
        .Where(sp => sp.Type == CapabilityStatement.SearchParamType.Reference)
        .Select(sp => sp.Name);

    supportedResource.SearchParam = new List<CapabilityStatement.CapabilityStatementRestResourceSearchParamComponent>();


    var parameters = ModelInfo.SearchParameters.Where(sp => sp.Resource == resourceName)
            .Select(sp => new CapabilityStatement.CapabilityStatementRestResourceSearchParamComponent
                {
                    Name = sp.Name,
                    Type = sp.Type,
                    Documentation = sp.Description,
                });

    supportedResource.SearchParam.AddRange(parameters);

    serverComponent.Resource.Add(supportedResource);
}
*/
// This constant has become internal. Please undo. We need it.

// Update: new location: XHtml.XHTMLNS / XHtml
//        // XNamespace ns = Hl7.Fhir.Support.Util.XHTMLNS;
//        XNamespace ns = "http://www.w3.org/1999/xhtml";

//        var summary = new XElement(ns + "div");

//        foreach (string resourceName in ModelInfo.SupportedResources)
//        {
//            summary.Add(new XElement(ns + "p",
//                String.Format("The server supports all operations on the {0} resource, including history",
//                    resourceName)));
//        }

//        capabilityStatement.Text = new Narrative();
//        capabilityStatement.Text.Div = summary.ToString();
//        return capabilityStatement;
//    }

//    public static string ReadVersionFromAssembly()
//    {
//        var attribute = (System.Reflection.AssemblyFileVersionAttribute)typeof(CapabilityStatementBuilder).Assembly
//            .GetCustomAttributes(typeof(System.Reflection.AssemblyFileVersionAttribute), true)
//            .Single();
//        return attribute.Version;
//    }

//    public static string ReadProductNameFromAssembly()
//    {
//        var attribute = (System.Reflection.AssemblyProductAttribute)typeof(CapabilityStatementBuilder).Assembly
//            .GetCustomAttributes(typeof(System.Reflection.AssemblyProductAttribute), true)
//            .Single();
//        return attribute.Product;
//    }
//}

//}