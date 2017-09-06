#region Information

// Solution:  Spark
// Spark.Engine
// File:  XmlFhirFormatter.cs
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
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using Core;
    using Engine.Auxiliary;
    using Engine.Core;
    using Engine.Extensions;
    using Hl7.Fhir.Model;
    using Hl7.Fhir.Rest;
    using Hl7.Fhir.Serialization;
    using Task = System.Threading.Tasks.Task;

    public class XmlFhirFormatter : FhirMediaTypeFormatter
    {
        public XmlFhirFormatter()
        {
            foreach (var mediaType in ContentType.XML_CONTENT_HEADERS)
                SupportedMediaTypes.Add(new MediaTypeHeaderValue(mediaType));
        }

        public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers,
            MediaTypeHeaderValue mediaType)
        {
            base.SetDefaultContentHeaders(type, headers, mediaType);
            headers.ContentType = FhirMediaType.GetMediaTypeHeaderValue(type, ResourceFormat.Xml);
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content,
            IFormatterLogger formatterLogger)
        {
            return Task.Factory.StartNew<object>(() =>
            {
                try
                {
                    var body = ReadBodyFromStream(readStream, content);

                    if (type == typeof(Bundle))
                        if (XmlSignatureHelper.IsSigned(body))
                            if (!XmlSignatureHelper.VerifySignature(body))
                                throw Error.BadRequest("Digital signature in body failed verification");

                    if (typeof(Resource).IsAssignableFrom(type))
                    {
#pragma warning disable 618
                        var resource = FhirParser.ParseResourceFromXml(body);
#pragma warning restore 618
                        return resource;
                    }
                    throw Error.Internal("The type {0} expected by the controller can not be deserialized", type.Name);
                }
                catch (FormatException exc)
                {
                    throw Error.BadRequest("Body parsing failed: " + exc.Message);
                }
            });
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content,
            TransportContext transportContext)
        {
            return Task.Factory.StartNew(() =>
            {
                XmlWriter writer = new XmlTextWriter(writeStream, new UTF8Encoding(false));
                var summary = RequestMessage.RequestSummary();

                if (type == typeof(OperationOutcome))
                {
                    var resource = (Resource) value;
                    FhirSerializer.SerializeResource(resource, writer, summary);
                }
                else if (typeof(Resource).IsAssignableFrom(type))
                {
                    var resource = (Resource) value;
                    FhirSerializer.SerializeResource(resource, writer, summary);
                }
                else if (type == typeof(FhirResponse))
                {
                    var response = value as FhirResponse;
                    if (response.HasBody)
                        FhirSerializer.SerializeResource(response.Resource, writer, summary);
                }

                writer.Flush();
            });
        }
    }
}