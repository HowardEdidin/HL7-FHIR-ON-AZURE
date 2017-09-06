#region Information

// Solution:  Spark
// FhirOnAzure
// File:  Global.asax.cs
//
// Created: 07/12/2017 : 10:35 AM
//
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:45 PM

#endregion

namespace FhirOnAzure
{
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using Engine.Extensions;

    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(Configure);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        public void Configure(HttpConfiguration config)
        {
            UnityConfig.RegisterComponents(config);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            config.AddFhir();
        }


    }
}