#region Information

// Solution:  Spark
// FhirOnAzure
// File:  FilterConfig.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:40 PM

#endregion

namespace FhirOnAzure
{
    using System.Web.Mvc;

    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ErrorHandler.AiHandleErrorAttribute());
        }
    }
}