#region Information

// Solution:  Spark
// Spark.Engine
// File:  FhirMediaTypeFormatter.cs
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
    using System.Text;
    using Core;
    using Engine.Core;
    using Engine.Extensions;
    using Hl7.Fhir.Model;

    public abstract class FhirMediaTypeFormatter : MediaTypeFormatter
    {
        protected Entry Entry;
        protected HttpRequestMessage RequestMessage;

        protected FhirMediaTypeFormatter()
        {
            SupportedEncodings.Clear();
            SupportedEncodings.Add(Encoding.UTF8);
        }

        private void SetEntryHeaders(HttpContentHeaders headers)
        {
            if (Entry == null) return;
            headers.LastModified = Entry.When;
            // todo: header.contentlocation
            //headers.ContentLocation = entry.Key.ToUri(Localhost.Base); dit moet door de exporter gezet worden.

            var resource = Entry.Resource as Binary;
            if (resource == null) return;
            var binary = resource;
            headers.ContentType = new MediaTypeHeaderValue(binary.ContentType);
        }

        public override bool CanReadType(Type type)
        {
            var can = typeof(Resource)
                .IsAssignableFrom(type); /* || type == typeof(Bundle) || (type == typeof(TagList) ) */
            return can;
        }

        public override bool CanWriteType(Type type)
        {
            return typeof(Resource).IsAssignableFrom(type);
        }

        public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers,
            MediaTypeHeaderValue mediaType)
        {
            base.SetDefaultContentHeaders(type, headers, mediaType);
            SetEntryHeaders(headers);
        }

        public override MediaTypeFormatter GetPerRequestFormatterInstance(Type type, HttpRequestMessage request,
            MediaTypeHeaderValue mediaType)
        {
            Entry = request.GetEntry();
            RequestMessage = request;
            return base.GetPerRequestFormatterInstance(type, request, mediaType);
        }

        protected string ReadBodyFromStream(Stream readStream, HttpContent content)
        {
            var charset = content.Headers.ContentType.CharSet ?? Encoding.UTF8.HeaderName;
            var encoding = Encoding.GetEncoding(charset);

            if (encoding != Encoding.UTF8)
                throw Error.BadRequest("FHIR supports UTF-8 encoding exclusively, not " + encoding.WebName);

            var reader = new StreamReader(readStream, Encoding.UTF8, true);
            return reader.ReadToEnd();
        }
    }
}