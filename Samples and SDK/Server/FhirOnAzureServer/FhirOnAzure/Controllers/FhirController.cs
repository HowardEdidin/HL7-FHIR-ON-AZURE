MIT License

Copyright (c) 2017 Howard Edidin

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.



#region Information

// Solution:  Spark
// FhirOnAzure
// File:  FhirController.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:41 PM

#endregion


namespace FhirOnAzure.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Cors;
    using Engine.Core;
    using Engine.Extensions;
    using Hl7.Fhir.Model;
    using Hl7.Fhir.Rest;
    using Infrastructure;
    using Microsoft.Practices.Unity;
    using Service;
    using Swashbuckle.Swagger.Annotations;
    using TRex.Metadata;

    [RoutePrefix("fhir")]
    [EnableCors("*", "*", "*", "*")]
    [RouteDataValuesOnly]
    // ReSharper disable once ClassTooBig
    public class FhirController : ApiController
    {
        private readonly IFhirService _fhirService;

        [InjectionConstructor]
        public FhirController(IFhirService fhirService)
        {
            // This will be a (injected) constructor parameter in ASP.vNext.
            _fhirService = fhirService;
        }

        [HttpGet]
        [Route("{type}/{id}")]
        [Metadata("Get", "Gets a Resource by type and id")]
        [SwaggerOperation("Read Resource", Tags = new[] {"Resource"})]
        public FhirResponse Read(string type, string id)
        {
            var parameters = new ConditionalHeaderParameters(Request);
            var key = Key.Create(type, id);
            var response = _fhirService.Read(key, parameters);

            return response;
        }

        [HttpGet]
        [Route("{type}/{id}/_history/{vid}")]
        [Metadata("Get", "Gets the Resource History by type, id, and History id")]
        [SwaggerOperation("Read Resource History", Tags = new[] {"Resource"})]
        public FhirResponse VRead(string type, string id, string vid)
        {
            var key = Key.Create(type, id, vid);
            return _fhirService.VersionRead(key);
        }

        [HttpPut]
        [Route("{type}/{id?}")]
        [Metadata("Put", "Updates a Resource by type and id")]
        [SwaggerOperation("Update Resource", Tags = new[] {"Resource"})]
        public FhirResponse Update(string type, Resource resource, string id = null)
        {
            var versionid = Request.IfMatchVersionId();
            var key = Key.Create(type, id, versionid);
            if (key.HasResourceId())
                return _fhirService.Update(key, resource);
            return _fhirService.ConditionalUpdate(key, resource,
                SearchParams.FromUriParamList(Request.TupledParameters()));
        }

        [HttpPost]
        [Route("{type}")]
        [Metadata("Create", "Creates a Resource by type")]
        [SwaggerOperation("Delete Resource", Tags = new[] {"Resource"})]
        public FhirResponse Create(string type, Resource resource)
        {
            var key = Key.Create(type, resource?.Id);

            if (!Request.Headers.Exists(FhirHttpHeaders.IfNoneExist)) return _fhirService.Create(key, resource);
            var searchQueryString =
                HttpUtility.ParseQueryString(
                    Request.Headers.First(h => h.Key == FhirHttpHeaders.IfNoneExist).Value.Single());
            var searchValues =
                searchQueryString.Keys.Cast<string>()
                    .Select(k => new Tuple<string, string>(k, searchQueryString[k]));


            return _fhirService.ConditionalCreate(key, resource, SearchParams.FromUriParamList(searchValues));

            //entry.Tags = Request.GetFhirTags(); // todo: move to model binder?
        }

        [HttpDelete]
        [Route("{type}/{id}")]
        [Metadata("Delete", "Deletes a Resource by type and id")]
        [SwaggerOperation("Delete Resource", Tags = new[] {"Resource"})]
        public FhirResponse Delete(string type, string id)

        {
            var key = Key.Create(type, id);
            var response = _fhirService.Delete(key);
            return response;
        }

        [HttpDelete]
        [Route("{type}")]
        [SwaggerOperation("Delete Resource By Type", Tags = new[] {"Resource"})]
        public FhirResponse ConditionalDelete(string type)
        {
            var key = Key.Create(type);
            return _fhirService.ConditionalDelete(key, Request.TupledParameters());
        }

        [HttpGet]
        [Route("{type}/{id}/_history")]
        [Metadata("Get History", "Get the history for Resource by type and id")]
        [SwaggerOperation("Get Resource History", Tags = new[] {"Resource"})]
        public FhirResponse History(string type, string id)
        {
            var key = Key.Create(type, id);
            var parameters = new HistoryParameters(Request);
            return _fhirService.History(key, parameters);
        }

        // ============= Validate
        [HttpPost]
        [Route("{type}/{id}/$validate")]
        [Metadata("ValidateResource", "Validate Resource a Resource by type and id")]
        [SwaggerOperation("Validate Resource", Tags = new[] {"Validation"})]
        public FhirResponse Validate(string type, string id, Resource resource)
        {
            //entry.Tags = Request.GetFhirTags();
            var key = Key.Create(type, id);
            return _fhirService.ValidateOperation(key, resource);
        }

        [HttpPost]
        [Route("{type}/$validate")]
        [Metadata("ValidateResource", "Validate Resource a Resource by type")]
        [SwaggerOperation("Validate Resource Type", Tags = new[] {"Validation"})]
        public FhirResponse Validate(string type, Resource resource)
        {
            // DSTU2: tags
            //entry.Tags = Request.GetFhirTags();
            var key = Key.Create(type);
            return _fhirService.ValidateOperation(key, resource);
        }

        // ============= Type Level Interactions

        [HttpGet]
        [Route("{type}")]
        [Metadata("Find Resources", "Searches for resources by type")]
        [SwaggerOperation("Search With Parameters", Tags = new[] {"Resource Type"})]
        public FhirResponse Search(string type)
        {
            var start = Request.GetIntParameter(FhirParameter.SNAPSHOT_INDEX) ?? 0;
            var searchparams = Request.GetSearchParams();
            //int pagesize = Request.GetIntParameter(FhirParameter.COUNT) ?? Const.DEFAULT_PAGE_SIZE;
            //string sortby = Request.GetParameter(FhirParameter.SORT);

            return _fhirService.Search(type, searchparams, start);
        }

        [HttpPost]
        [HttpGet]
        [Route("{type}/_search")]
        [Metadata("Find Resources", "Searches for resources by type")]
        [SwaggerOperation("Search By Type", Tags = new[] {"Resource Type"})]
        public FhirResponse SearchWithOperator(string type)
        {
            // todo: get tupled parameters from post.
            return Search(type);
        }

        [HttpGet]
        [Route("{type}/_history")]
        [Metadata("Get History", "Get the history of Resources by type")]
        [SwaggerOperation("Get History By Type", Tags = new[] {"Resource Type"})]
        public FhirResponse History(string type)
        {
            var parameters = new HistoryParameters(Request);
            return _fhirService.History(type, parameters);
        }

        // ============= Whole System Interactions

        [HttpGet]
        [Route("metadata")]
        [Metadata("GetMetaData", "Get the Capability Statement")]
        [SwaggerOperation("Get MetaData", Tags = new[] {"System Metadata"})]
        public FhirResponse Metadata()
        {
            return _fhirService.CapabilityStatement(Settings.Version);
        }

        //[HttpOptions]
        //[Route("")]
        //[Metadata("GetMetaData", "Get the Capability Statement for the server")]
        //[SwaggerOperation("Get Options", Tags = new[] { "System Capability" })]
        //public FhirResponse Options()
        //{
        //    return _fhirService.CapabilityStatement(Settings.Version);
        //}

        [HttpPost]
        [Route("")]
        [Metadata("Transaction", "Perform Operation")]
        [SwaggerOperation("Perform Operation", Tags = new[] {"System Transaction"})]
        public FhirResponse Transaction(Bundle bundle)
        {
            return _fhirService.Transaction(bundle);
        }


        [HttpGet]
        [Route("_history")]
        [Metadata("History", "Get System History")]
        [SwaggerOperation("Get System History", Tags = new[] {"System"})]
        public FhirResponse History()
        {
            var parameters = new HistoryParameters(Request);
            return _fhirService.History(parameters);
        }

        [HttpGet]
        [Route("_snapshot")]
        [Metadata("Snapshot", "Get System Snapshot")]
        [SwaggerOperation("Get Snapshot", Tags = new[] {"System"})]
        public FhirResponse Snapshot()
        {
            var snapshot = Request.GetParameter(FhirParameter.SNAPSHOT_ID);
            var start = Request.GetIntParameter(FhirParameter.SNAPSHOT_INDEX) ?? 0;
            return _fhirService.GetPage(snapshot, start);
        }


        [HttpPost]
        [Route("{type}/{id}/${operation}")]
        [Metadata("InstanceOperation", "Run Instance Operation")]
        [SwaggerOperation("Run Instance Operation", Tags = new[] {"System"})]
        public FhirResponse InstanceOperation(string type, string id, string operation, Parameters parameters)
        {
            var key = Key.Create(type, id);
            switch (operation.ToLower())
            {
                case "meta": return _fhirService.ReadMeta(key);
                case "meta-add": return _fhirService.AddMeta(key, parameters);
                // ReSharper disable once RedundantCaseLabel
                case "meta-delete":

                default: return Respond.WithError(HttpStatusCode.NotFound, "Unknown operation");
            }
        }

        [HttpPost]
        [HttpGet]
        [Route("{type}/{id}/$everything")]
        [Metadata("Everything", "Get Everything By Type - optional id")]
        [SwaggerOperation("Get Everything", Tags = new[] {"System"})]
        // ReSharper disable once MethodOverloadWithOptionalParameter
        public FhirResponse Everything(string type, string id = null)
        {
            var key = Key.Create(type, id);
            return _fhirService.Everything(key);
        }

        [HttpPost]
        [HttpGet]
        [Route("{type}/$everything")]
        [Metadata("Everything", "Get Everything By Type")]
        [SwaggerOperation("Get Everything By Type", Tags = new[] {"System"})]
        public FhirResponse Everything(string type)
        {
            var key = Key.Create(type);
            return _fhirService.Everything(key);
        }

        [HttpPost]
        [HttpGet]
        [Route("Composition/{id}/$document")]
        [Metadata("Document", "Get a Composition Document")]
        [SwaggerOperation("Get Composition Document", Tags = new[] {"System"})]
        public FhirResponse Document(string id)
        {
            var key = Key.Create("Composition", id);
            return _fhirService.Document(key);
        }

      
    }
}
