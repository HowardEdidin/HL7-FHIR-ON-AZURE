#region Information

// Solution:  Spark
// Spark.Engine
// File:  MediaTypeHandler.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:03 PM

#endregion

namespace FhirOnAzure.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using Engine.Core;
    using Engine.Extensions;
    using Hl7.Fhir.Rest;

    public class FhirMediaTypeHandler : DelegatingHandler
    {
        private bool isBinaryRequest(HttpRequestMessage request)
        {
            var ub = new UriBuilder(request.RequestUri);
            return ub.Path.Contains("Binary");
            // HACK: replace quick hack by solid solution.
        }

        private bool isTagRequest(HttpRequestMessage request)
        {
            var ub = new UriBuilder(request.RequestUri);
            return ub.Path.Contains("_tags");
            // HACK: replace quick hack by solid solution.
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var formatParam = request.GetParameter("_format");
            if (!string.IsNullOrEmpty(formatParam))
            {
                var accepted = ContentType.GetResourceFormatFromFormatParam(formatParam);
                if (accepted != ResourceFormat.Unknown)
                {
                    request.Headers.Accept.Clear();

                    if (accepted == ResourceFormat.Json)
                        request.Headers.Accept.Add(
                            new MediaTypeWithQualityHeaderValue(ContentType.JSON_CONTENT_HEADER));
                    else
                        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType.XML_CONTENT_HEADER));
                }
            }

            // BALLOT: binary upload should be determined by the Content-Type header, instead of the Rest url?
            // HACK: passes to BinaryFhirFormatter
            if (isBinaryRequest(request))
            {
                if (request.Content.Headers.ContentType != null)
                {
                    var format = request.Content.Headers.ContentType.MediaType;
                    request.Content.Headers.Replace("X-Content-Type", format);
                }

                request.Content.Headers.ContentType = new MediaTypeHeaderValue(FhirMediaType.BinaryResource);
                if (request.Headers.Accept.Count == 0)
                    request.Headers.Replace("Accept", FhirMediaType.BinaryResource);
            }

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }

    // Instead of using the general purpose DelegatingHandler, could we use IContentNegotiator?
    public class FhirContentNegotiator : IContentNegotiator
    {
        public ContentNegotiationResult Negotiate(Type type, HttpRequestMessage request,
            IEnumerable<MediaTypeFormatter> formatters)
        {
            throw new NotImplementedException();
        }
    }
}