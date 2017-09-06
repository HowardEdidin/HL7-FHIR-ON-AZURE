#region Information

// Solution:  Spark
// FhirOnAzure
// File:  HomeController.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:42 PM

#endregion

namespace FhirOnAzure.Controllers
{
    using System.Web.Mvc;
    using MetaStore;
    using MongoDB.Driver;
    using Store.Mongo;

    public class HomeController : Controller
    {
        //private string mongoUrl;
        //[Dependency]
        //public string MongoUrl { private get { return mongoUrl; } set { mongoUrl = value; } }

        private readonly MongoDatabase _db;

        public HomeController(string mongoUrl)
        {
            _db = MongoDatabaseFactory.GetMongoDatabase(mongoUrl);
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult Overview()
        {
            var store = new MetaContext(_db);
            var stats = new VmStatistics {ResourceStats = store.GetResourceStats()};

            return View(stats);
        }
    }
}