#region Information

// Solution:  Spark
// Spark.Engine
// File:  Response.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:12 PM

#endregion

namespace FhirOnAzure.Engine.Core
{
    using System.Net;
    using System.Net.Http;
    using Hl7.Fhir.Model;

    // THe response class is an abstraction of the Fhir REST responses
    // This way, it's easier to implement multiple WebApi controllers
    // without having to implement functionality twice.
    // The FhirService always responds with a "Response"

    public class RespTest : HttpResponseMessage
    {
    }

    public class FhirResponse
    {
        public IKey Key;
        public Resource Resource;
        public HttpStatusCode StatusCode;

        public FhirResponse(HttpStatusCode code, IKey key, Resource resource)
        {
            StatusCode = code;
            Key = key;
            Resource = resource;
        }

        public FhirResponse(HttpStatusCode code, Resource resource)
        {
            StatusCode = code;
            Key = null;
            Resource = resource;
        }

        public FhirResponse(HttpStatusCode code)
        {
            StatusCode = code;
            Key = null;
            Resource = null;
        }

        public bool IsValid
        {
            get
            {
                var code = (int) StatusCode;
                return code <= 300;
            }
        }

        public bool HasBody => Resource != null;

        public override string ToString()
        {
            var details = Resource != null ? $"({Resource.TypeName})" : null;
            var location = Key?.ToString();
            return $"{(int) StatusCode}: {StatusCode} {details} ({location})";
        }
    }
}