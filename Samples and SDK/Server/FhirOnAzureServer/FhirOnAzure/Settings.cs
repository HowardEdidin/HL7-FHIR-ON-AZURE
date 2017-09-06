#region Information

// Solution:  Spark
// FhirOnAzure
// File:  Settings.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:45 PM

#endregion


namespace FhirOnAzure
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Web.Hosting;

    public static class Settings
    {
        public static string Version
        {
            get
            {
                var asm = Assembly.GetExecutingAssembly();
                // ReSharper disable once AssignNullToNotNullAttribute
                var version = FileVersionInfo.GetVersionInfo(asm.Location);
                return $"{version.ProductMajorPart}.{version.ProductMinorPart}";
            }
        }

        public static bool UseS3
        {
            get
            {
                try
                {
                    var useS3 = GetRequiredKey("FHIR_USE_S3");
                    return useS3 == "true";
                }
                catch
                {
                    return false;
                }
            }
        }

        public static int MaxBinarySize
        {
            get
            {
                try
                {
                    int max = Convert.ToInt16(GetRequiredKey("MaxBinarySize"));
                    if (max == 0) max = short.MaxValue;
                    return max;
                }
                catch
                {
                    return short.MaxValue;
                }
            }
        }

        public static string ApiEndpoint => GetRequiredKey("FHIR_API");


        public static string MongoUrl => GetRequiredKey("MONGOLAB_URI");

        public static Uri Swagger
        {
            get
            {
                var api = GetRequiredKey("Swagger");
                return new Uri(api, UriKind.Absolute);
            }
        }

        public static Uri Endpoint
        {
            get
            {
                var endpoint = GetRequiredKey("FHIR_ENDPOINT");
                return new Uri(endpoint, UriKind.Absolute);
            }
        }

        public static string AuthorUri => Endpoint.Host;

        public static string ExamplesFile
        {
            get
            {
                var path = HostingEnvironment.ApplicationPhysicalPath;

                if (string.IsNullOrEmpty(path))
                    path = ".";

                return Path.Combine(path, "files", "examples.zip");
            }
        }

        public static long MaximumDecompressedBodySizeInBytes => long.Parse(
            GetRequiredKey("MaxDecompressedBodySizeInBytes"));


        private static string GetRequiredKey(string key)
        {
            var s = ConfigurationManager.AppSettings.Get(key);

            if (string.IsNullOrEmpty(s))
                throw new ArgumentException($"The configuration variable {key} is missing.");

            return s;
        }
    }
}