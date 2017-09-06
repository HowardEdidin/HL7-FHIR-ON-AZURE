using System.Linq;
using System.Net;
using FhirOnAzure.Engine.Core;
using FhirOnAzure.Engine.Extensions;
using FhirOnAzure.Engine.Interfaces;

namespace FhirOnAzure.Engine.FhirResponseFactory
{
    public class ConditionalHeaderFhirResponseInterceptor : IFhirResponseInterceptor
    {
        public bool CanHandle(object input)
        {
            return input is ConditionalHeaderParameters;
        }

        private ConditionalHeaderParameters ConvertInput(object input)
        {
            return input as ConditionalHeaderParameters;
        }

        public FhirResponse GetFhirResponse(Entry entry, object input)
        {
            ConditionalHeaderParameters parameters = ConvertInput(input);
            if (parameters == null) return null;

            bool? matchTags = parameters.IfNoneMatchTags.Any() ? parameters.IfNoneMatchTags.Any(t => t == ETag.Create(entry.Key.VersionId).Tag) : (bool?)null;
            bool? matchModifiedDate = parameters.IfModifiedSince.HasValue
                ? parameters.IfModifiedSince.Value < entry.Resource.Meta.LastUpdated
                : (bool?) null;

            if (!matchTags.HasValue  && !matchModifiedDate.HasValue)
            {
                return null;
            }

            if ((matchTags ?? true) && (matchModifiedDate ?? true))
            {
                return Respond.WithCode(HttpStatusCode.NotModified);
            }

            return null;
        }
    }
}