#region Information

// Solution:  Spark
// Spark.Engine
// File:  JsonFhirFormatter.cs
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
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Core;
    using Engine.Core;
    using Engine.Extensions;
    using Hl7.Fhir.Model;
    using Hl7.Fhir.Rest;
    using Hl7.Fhir.Serialization;
    using Newtonsoft.Json;
    using Task = System.Threading.Tasks.Task;

    public class JsonFhirFormatter : FhirMediaTypeFormatter
    {
        public JsonFhirFormatter()
        {
            foreach (var mediaType in ContentType.JSON_CONTENT_HEADERS)
                SupportedMediaTypes.Add(new MediaTypeHeaderValue(mediaType));
        }

        public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers,
            MediaTypeHeaderValue mediaType)
        {
            base.SetDefaultContentHeaders(type, headers, mediaType);
            headers.ContentType = FhirMediaType.GetMediaTypeHeaderValue(type, ResourceFormat.Json);
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content,
            IFormatterLogger formatterLogger)
        {
            return Task.Factory.StartNew<object>(() =>
            {
                try
                {
                    var body = ReadBodyFromStream(readStream, content);

                    if (typeof(Resource).IsAssignableFrom(type))
                    {
                        var resource = FhirParser.ParseResourceFromJson(body);
                        return resource;
                    }
                    throw Error.Internal("Cannot read unsupported type {0} from body", type.Name);
                }
                catch (FormatException exception)
                {
                    throw Error.BadRequest("Body parsing failed: " + exception.Message);
                }
            });
        }

        /// <inheritdoc />
        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content,
            TransportContext transportContext)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var streamwriter = new StreamWriter(writeStream))
                using (JsonWriter writer = new JsonTextWriter(streamwriter))
                {
                    var summary = RequestMessage.RequestSummary();

                    if (type == typeof(OperationOutcome))
                    {
                        var resource = (Resource) value;
                        FhirSerializer.SerializeResource(resource, writer);
                    }
                    else if (typeof(Resource).IsAssignableFrom(type))
                    {
                        var resource = (Resource) value;
                        FhirSerializer.SerializeResource(resource, writer);
                    }
                    else if (typeof(FhirResponse).IsAssignableFrom(type))
                    {
                        var response = value as FhirResponse;
                        if (response != null && response.HasBody)
                            FhirSerializer.SerializeResource(response.Resource, writer, summary);
                    }
                }
            });
        }
    }
}