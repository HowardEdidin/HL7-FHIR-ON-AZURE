#region Information

// Solution:  Spark
// FhirOnAzure
// File:  MaintenanceController.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:42 PM

#endregion

namespace FhirOnAzure.Controllers
{
    using System.Web.Mvc;

    public class MaintenanceController : Controller
    {
        // GET: Maintenance
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Initialize()
        {
            return View();
        }
    }
}