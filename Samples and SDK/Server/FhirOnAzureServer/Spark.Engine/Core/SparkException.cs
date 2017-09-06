#region Information

// Solution:  Spark
// Spark.Engine
// File:  SparkException.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:12 PM

#endregion

namespace FhirOnAzure.Engine.Core
{
    using System;
    using System.Net;
    using Hl7.Fhir.Model;

    // Placed in a sub-namespace because you must be explicit about it if you want to throw this error directly

    // todo: Can this be replaced by a FhirOperationException ?

    public class SparkException : Exception
    {
        public HttpStatusCode StatusCode;

        public SparkException(HttpStatusCode statuscode, string message = null) : base(message)
        {
            StatusCode = statuscode;
        }

        public SparkException(HttpStatusCode statuscode, string message, params object[] values)
            : base(string.Format(message, values))
        {
            StatusCode = statuscode;
        }

        public SparkException(string message) : base(message)
        {
            StatusCode = HttpStatusCode.BadRequest;
        }

        public SparkException(HttpStatusCode statuscode, string message, Exception inner) : base(message, inner)
        {
            StatusCode = statuscode;
        }

        public SparkException(HttpStatusCode statuscode, OperationOutcome outcome, string message = null)
            : this(statuscode, message)
        {
            Outcome = outcome;
        }

        public OperationOutcome Outcome { get; set; }
    }
}