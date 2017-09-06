#region Information

// Solution:  Spark
// Spark.Engine
// File:  UntypedValue.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:01 PM

#endregion

namespace FhirOnAzure.Search
{
    using Hl7.Fhir.Model;

    public class UntypedValue : ValueExpression
    {
        public UntypedValue(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public override string ToString()
        {
            return Value;
        }

        public NumberValue AsNumberValue()
        {
            return NumberValue.Parse(Value);
        }

        public DateValue AsDateValue()
        {
            return DateValue.Parse(Value);
        }

        public FhirDateTime AsDateTimeValue()
        {
            return new FhirDateTime(Value);
        }

        public StringValue AsStringValue()
        {
            return StringValue.Parse(Value);
        }

        public TokenValue AsTokenValue()
        {
            return TokenValue.Parse(Value);
        }

        public QuantityValue AsQuantityValue()
        {
            return QuantityValue.Parse(Value);
        }

        public ReferenceValue AsReferenceValue()
        {
            return ReferenceValue.Parse(Value);
        }
    }
}