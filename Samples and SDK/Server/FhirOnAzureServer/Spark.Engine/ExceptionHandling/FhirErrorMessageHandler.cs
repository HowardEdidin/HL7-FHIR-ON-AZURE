#region Information

// Solution:  Spark
// Spark.Engine
// File:  FhirErrorMessageHandler.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:51 PM

#endregion

namespace FhirOnAzure.Engine.ExceptionHandling
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Extensions;
    using Hl7.Fhir.Model;

    public class FhirErrorMessageHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var content = response.Content as ObjectContent;
                if (content != null && content.ObjectType == typeof(HttpError))
                {
                    var outcome = new OperationOutcome().AddError(response.ReasonPhrase);
                    return request.CreateResponse(response.StatusCode, outcome);
                }
            }
            return response;
        }
    }
}