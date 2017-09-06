using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FhirOnAzure.Engine.Core;
using FhirOnAzure.Engine.Extensions;

namespace FhirOnAzure.Core
{

    public class FhirResponseHandler : DelegatingHandler
    {

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken).ContinueWith(
                task =>
                {
                    FhirResponse fhirResponse;
                    if (task.IsCompleted)
                    {
                        if (task.Result.TryGetContentValue(out fhirResponse))
                        {
                            return request.CreateResponse(fhirResponse);
                        }
                        else
                        {
                            return task.Result;
                        }
                    } 
                    else
                    {
                        return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
                        //return task.Result;
                    }
                    
                }, 
                cancellationToken
            );
             
        }

    }

    
}
