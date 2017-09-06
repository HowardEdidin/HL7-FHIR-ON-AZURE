#region Information

// Solution:  Spark
// FhirOnAzure
// File:  BundleConfig.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:40 PM

#endregion

namespace FhirOnAzure
{
    using System.Web.Optimization;

    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Content/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Content/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Content/Scripts/bootstrap.js",
                "~/Content/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                "~/Content/css/bootstrap.css",
                "~/Content/css/site.css"));
        }
    }
}