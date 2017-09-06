﻿#region Information

// Solution:  Spark
// Spark.Engine
// File:  BinaryFormatter.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:04 PM

#endregion

namespace FhirOnAzure.Formatters
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Core;
    using Engine.Core;
    using Hl7.Fhir.Model;
    using Task = System.Threading.Tasks.Task;

    //public class MatchBinaryPathTypeMapping : MediaTypeMapping
    //{
    //    public MatchBinaryPathTypeMapping() : base("text/plain") { }

    //    private bool isBinaryRequest(HttpRequestMessage request)
    //    {
    //        return request.RequestUri.AbsolutePath.Contains("Binary"); // todo: replace quick hack by solid solution.
    //    }

    //    public override double TryMatchMediaType(HttpRequestMessage request)
    //    {
    //        return isBinaryRequest(request) ? 1.0f : 0.0f;
    //    }
    //}

    public class BinaryFhirFormatter : FhirMediaTypeFormatter
    {
        public BinaryFhirFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(FhirMediaType.BinaryResource));
            //     MediaTypeMappings.Add(new MatchBinaryPathTypeMapping());
        }

        public override bool CanReadType(Type type)
        {
            return type == typeof(Resource);
        }

        public override bool CanWriteType(Type type)
        {
            return type == typeof(Binary);
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content,
            IFormatterLogger formatterLogger)
        {
            return Task.Factory.StartNew(() =>
            {
                var stream = new MemoryStream();
                readStream.CopyTo(stream);

                IEnumerable<string> xContentHeader;
                var success = content.Headers.TryGetValues("X-Content-Type", out xContentHeader);

                if (!success)
                    throw Error.BadRequest("POST to binary must provide a Content-Type header");

                var contentType = xContentHeader.FirstOrDefault();

                var binary = new Binary
                {
                    Content = stream.ToArray(),
                    ContentType = contentType
                };

                //ResourceEntry entry = ResourceEntry.Create(binary);
                //entry.Tags = content.Headers.GetFhirTags();
                return (object) binary;
            });
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content,
            TransportContext transportContext)
        {
            return Task.Factory.StartNew(() =>
            {
                var binary = (Binary) value;
                //Binary binary = (Binary)entry.Resource;

                //content.Headers.ContentType = new MediaTypeHeaderValue(binary.ContentType);
                //content.Headers.Replace("Content-Type", binary.ContentType);  // todo: HACK on Binary content Type!!!

                var stream = new MemoryStream(binary.Content);

                stream.CopyTo(writeStream);

                stream.Flush();
            });
        }
    }
}