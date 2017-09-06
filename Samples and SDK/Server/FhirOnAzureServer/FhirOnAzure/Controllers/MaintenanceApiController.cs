#region Information

// Solution:  Spark
// FhirOnAzure
// File:  MaintenanceApiController.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:42 PM

#endregion

namespace FhirOnAzure.Controllers
{
    using System;
    using System.Configuration;
    using System.Web.Http;
    using Core;
    using Engine.Interfaces;

    [RoutePrefix("MaintenanceApi")]
    public class MaintenanceApiController : ApiController
    {
        private readonly IFhirIndex fhirIndex;
        private readonly IFhirStoreAdministration fhirStoreAdministration;

        public MaintenanceApiController(IFhirStoreAdministration fhirStoreAdministration, IFhirIndex fhirIndex)
        {
            this.fhirStoreAdministration = fhirStoreAdministration;
            this.fhirIndex = fhirIndex;
        }

        [HttpDelete]
        [Route("All")]
        public void ClearAll(Guid access)
        {
            var code = ConfigurationManager.AppSettings.Get("clearAllCode");
            if (!string.IsNullOrEmpty(code) && access.ToString() == code)
            {
                fhirStoreAdministration.Clean();
                fhirIndex.Clean();
            }
        }
    }
}