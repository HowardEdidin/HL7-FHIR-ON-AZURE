#region Information

// Solution:  Spark
// Spark.Engine
// File:  ReferenceValue.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:01 PM

#endregion

namespace FhirOnAzure.Search
{
    using System;
    using Hl7.Fhir.Model;
    using Support;

    public class ReferenceValue : ValueExpression
    {
        public ReferenceValue(string value)
        {
            if (!Uri.IsWellFormedUriString(value, UriKind.Absolute) &&
                !Id.IsValidValue(value))
                throw Error.Argument("text", "Reference is not a valid Id nor a valid absolute Url");

            Value = value;
        }

        public string Value { get; }

        public override string ToString()
        {
            return StringValue.EscapeString(Value);
        }

        public static ReferenceValue Parse(string text)
        {
            var value = StringValue.UnescapeString(text);

            return new ReferenceValue(value);
        }
    }
}