#region Information

// Solution:  Spark
// Spark.Engine
// File:  InterceptBodyHandler.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:03 PM

#endregion

namespace FhirOnAzure.Handlers
{
    //public class InterceptBodyHandler : DelegatingHandler
    //{
    //    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    //    {
    //        if (request.Content != null)
    //        {
    //            return request.Content.ReadAsByteArrayAsync().ContinueWith((task) =>
    //            {
    //                var data = task.Result;
    //                if (data != null && data.Length > 0)
    //                {
    //                    string mediaType = FhirMediaType.GetMediaType(request);

    //                    request.SaveBody(mediaType, task.Result);
    //                }
    //                return base.SendAsync(request, cancellationToken);
    //            }).Result;
    //        }
    //        else
    //            return base.SendAsync(request, cancellationToken);
    //    }
    //}
}