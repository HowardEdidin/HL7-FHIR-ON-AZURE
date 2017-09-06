#region Information

// Solution:  Spark
// Spark.Engine
// File:  FhirMediaType.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:11 PM

#endregion



//using System.Web.Http;

namespace FhirOnAzure.Engine.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using Hl7.Fhir.Model;
    using Hl7.Fhir.Rest;

    public static class FhirMediaType
    {
        // API: This class can be merged into HL7.Fhir.Rest.ContentType

        public const string XmlResource = "application/xml+fhir";
        public const string XmlTagList = "application/xml+fhir";

        public const string JsonResource = "application/json+fhir";
        public const string JsonTagList = "application/json+fhir";

        public const string BinaryResource = "application/fhir+binary";
        public static string[] LooseXmlFormats = {"xml", "text/xml", "application/xml"};
        public static readonly string[] LooseJsonFormats = {"json", "application/json"};

        public static ICollection<string> StrictFormats => new List<string> {XmlResource, JsonResource};

        /// <summary>
        ///     Transforms loose formats to their strict variant
        /// </summary>
        /// <param name="format">Mime type</param>
        /// <returns></returns>
        public static string Interpret(string format)
        {
            if (format == null) return XmlResource;
            if (StrictFormats.Contains(format)) return format;
            if (LooseXmlFormats.Contains(format)) return XmlResource;
            if (LooseJsonFormats.Contains(format)) return JsonResource;
            return format;
        }

        public static ResourceFormat GetResourceFormat(string format)
        {
            var strict = Interpret(format);
            if (strict == XmlResource) return ResourceFormat.Xml;
            if (strict == JsonResource) return ResourceFormat.Json;
            return ResourceFormat.Xml;
        }

        public static string GetContentType(Type type, ResourceFormat format)
        {
            if (typeof(Resource).IsAssignableFrom(type) || type == typeof(Resource))
                switch (format)
                {
                    case ResourceFormat.Json: return JsonResource;
                    case ResourceFormat.Xml: return XmlResource;
                    default: return XmlResource;
                }
            return "application/octet-stream";
        }

        public static string GetMediaType(this HttpRequestMessage request)
        {
            var headervalue = request.Content.Headers.ContentType;
            var s = headervalue != null ? headervalue.MediaType : null;
            return Interpret(s);
        }

        public static MediaTypeHeaderValue GetMediaTypeHeaderValue(Type type, ResourceFormat format)
        {
            var mediatype = GetContentType(type, format);
            var header = new MediaTypeHeaderValue(mediatype);
            header.CharSet = Encoding.UTF8.WebName;
            return header;
        }
    }
}