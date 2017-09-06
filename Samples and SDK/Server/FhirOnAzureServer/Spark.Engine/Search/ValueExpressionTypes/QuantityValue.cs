#region Information

// Solution:  Spark
// Spark.Engine
// File:  QuantityValue.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:01 PM

#endregion

namespace FhirOnAzure.Search
{
    using System;
    using Hl7.Fhir.Serialization;
    using Support;

    public class QuantityValue : ValueExpression
    {
        public QuantityValue(decimal number, string unit)
        {
            Number = number;
            Unit = unit;
        }

        public QuantityValue(decimal number, string ns, string unit)
        {
            Number = number;
            Unit = unit;
            Namespace = ns;
        }

        public decimal Number { get; }

        public string Namespace { get; }

        public string Unit { get; }

        public override string ToString()
        {
            var ns = Namespace ?? string.Empty;
            return PrimitiveTypeConverter.ConvertTo<string>(Number) + "|" +
                   StringValue.EscapeString(ns) + "|" +
                   StringValue.EscapeString(Unit);
        }

        public static QuantityValue Parse(string text)
        {
            if (text == null) throw Error.ArgumentNull("text");

            var triple = text.SplitNotEscaped('|');

            if (triple.Length != 3)
                throw Error.Argument("text", "Quantity needs to have three parts separated by '|'");

            if (triple[0] == string.Empty)
                throw new FormatException("Quantity needs to specify a number");

            var number = PrimitiveTypeConverter.ConvertTo<decimal>(triple[0]);
            var ns = triple[1] != string.Empty ? StringValue.UnescapeString(triple[1]) : null;

            if (triple[2] == string.Empty)
                throw new FormatException("Quantity needs to specify a unit");

            var unit = StringValue.UnescapeString(triple[2]);

            return new QuantityValue(number, ns, unit);
        }
    }
}