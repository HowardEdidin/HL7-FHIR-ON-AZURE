#region Information

// Solution:  Spark
// FhirOnAzure
// File:  RouteDataValuesOnlyAttribute.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:44 PM

#endregion

namespace FhirOnAzure.Infrastructure
{
    using System;
    using System.Web.Http.Controllers;
    using System.Web.Http.ValueProviders;
    using System.Web.Http.ValueProviders.Providers;

    //Inspiration: http://www.strathweb.com/2013/04/asp-net-web-api-and-greedy-query-string-parameter-binding/
    public class RouteDataValuesOnlyAttribute : Attribute, IControllerConfiguration
    {
        public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            controllerSettings.Services.Replace(typeof(ValueProviderFactory), new RouteDataValueProviderFactory());
        }
    }
}