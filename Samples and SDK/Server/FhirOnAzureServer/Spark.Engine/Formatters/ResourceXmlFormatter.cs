#region Information

// Solution:  Spark
// Spark.Engine
// File:  ResourceXmlFormatter.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:04 PM

#endregion

namespace FhirOnAzure.Formatters
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Hl7.Fhir.Model;
    using Task = System.Threading.Tasks.Task;

    public class ResourceXmlFormatter : MediaTypeFormatter
    {
        public ResourceXmlFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/xml"));
        }

        public override bool CanReadType(Type type)
        {
            return type == typeof(Resource);
        }

        public override bool CanWriteType(Type type)
        {
            return false;
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content,
            IFormatterLogger formatterLogger)
        {
            return Task.Factory.StartNew(() => (object)null);
        }
    }
}