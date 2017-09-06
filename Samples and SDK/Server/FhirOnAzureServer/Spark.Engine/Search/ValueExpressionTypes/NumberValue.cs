#region Information

// Solution:  Spark
// Spark.Engine
// File:  NumberValue.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:01 PM

#endregion

namespace FhirOnAzure.Search
{
    using Hl7.Fhir.Serialization;

    public class NumberValue : ValueExpression
    {
        public NumberValue(decimal value)
        {
            Value = value;
        }

        public decimal Value { get; }

        public override string ToString()
        {
            return PrimitiveTypeConverter.ConvertTo<string>(Value);
        }

        public static NumberValue Parse(string text)
        {
            return new NumberValue(PrimitiveTypeConverter.ConvertTo<decimal>(text));
        }
    }
}