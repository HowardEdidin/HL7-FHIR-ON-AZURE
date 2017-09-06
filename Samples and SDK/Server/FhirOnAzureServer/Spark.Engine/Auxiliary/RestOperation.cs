#region Information

// Solution:  Spark
// Spark.Engine
// File:  RestOperation.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:50 PM

#endregion

namespace FhirOnAzure.Engine.Auxiliary
{
    using Hl7.Fhir.Rest;

    public class RestOperation
    {
        // API: this constant can be derived from TransactionBuilder. 
        // But the History keyword has a bigger scope than just TransactionBuilder.
        // proposal: move HISTORY and other URL/operation constants to Hl7.Fhir.Rest.Operation or something.
        public static string HISTORY = TransactionBuilder.HISTORY;
    }
}