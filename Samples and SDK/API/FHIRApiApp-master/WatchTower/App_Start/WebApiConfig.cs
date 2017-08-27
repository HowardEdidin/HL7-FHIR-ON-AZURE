using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace WatchTower
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //Must have Cors enabled
            config.EnableCors();
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
