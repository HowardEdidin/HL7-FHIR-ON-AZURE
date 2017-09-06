#region Information

// Solution:  Spark
// Spark.Engine
// File:  ExceptionResponseMessageFactory.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:51 PM

#endregion

namespace FhirOnAzure.Engine.ExceptionHandling
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using Core;
    using Extensions;
    using Hl7.Fhir.Model;

    public class ExceptionResponseMessageFactory : IExceptionResponseMessageFactory
    {
        /*
                private SparkException _ex;
        */

        public HttpResponseMessage GetResponseMessage(Exception exception, HttpRequestMessage request)
        {
            if (exception == null)
                return null;

            var response = InternalCreateHttpResponseMessage(exception as SparkException, request) ??
                           InternalCreateHttpResponseMessage(exception as HttpResponseException, request) ??
                           InternalCreateHttpResponseMessage(exception, request);

            return response;
        }

        private HttpResponseMessage InternalCreateHttpResponseMessage(SparkException exception,
            HttpRequestMessage request)
        {
            if (exception == null)
                return null;

            var outcome = exception.Outcome ?? new OperationOutcome();
            outcome.AddAllInnerErrors(exception);
            return request.CreateResponse(exception.StatusCode, outcome);
        }

        private HttpResponseMessage InternalCreateHttpResponseMessage(HttpResponseException exception,
            HttpRequestMessage request)
        {
            if (exception == null)
                return null;

            var outcome = new OperationOutcome().AddError(exception.Response.ReasonPhrase);
            return request.CreateResponse(exception.Response.StatusCode, outcome);
        }

        private HttpResponseMessage InternalCreateHttpResponseMessage(Exception exception, HttpRequestMessage request)
        {
            if (exception == null)
                return null;

            var outcome = new OperationOutcome().AddAllInnerErrors(exception);
            return request.CreateResponse(HttpStatusCode.InternalServerError, outcome);
        }
    }
}