#region Information

// Solution:  Spark
// Spark.Engine
// File:  DateTimeValue.cs
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

    /// <summary>
    ///     DateTimeValue is allways specified up to the second.
    ///     FhirOnAzure uses it for the boundaries of a period. So fuzzy dates as in FhirDateTime (just year + month for
    ///     example) get translated in an upper- and lowerbound in DateTimeValues.
    ///     These are used for indexing.
    /// </summary>
    public class DateTimeValue : ValueExpression
    {
        public DateTimeValue(DateTimeOffset value)
        {
            // The DateValue datatype is not interested in any time related
            // components, so we must strip those off before converting to the string
            // value
            Value = value;
        }

        public DateTimeValue(string datetime)
        {
            if (!FhirDateTime.IsValidValue(datetime))
                throw Error.Argument("datetime",
                    "The string [" + datetime + "] cannot be translated to a DateTimeValue");
            var fdt = new FhirDateTime(datetime);
            Value = fdt.ToDateTimeOffset();
        }

        public DateTimeOffset Value { get; }

        public override string ToString()
        {
            return new FhirDateTime(Value).ToString();
            //return Value.ToString("YYYY-MM-ddThh:mm:sszzz");
        }

        public static DateTimeValue Parse(string text)
        {
            return new DateTimeValue(text);
        }
    }
}