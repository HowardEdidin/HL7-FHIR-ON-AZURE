#region Information

// Solution:  Spark
// Spark.Engine
// File:  DateValue.cs
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

    public class DateValue : ValueExpression
    {
        public DateValue(DateTimeOffset value)
        {
            // The DateValue datatype is not interested in any time related
            // components, so we must strip those off before converting to the string
            // value
            Value = value.Date.ToString("yyyy-MM-dd");
        }

        public DateValue(string date)
        {
            if (!Date.IsValidValue(date))
            {
                if (!FhirDateTime.IsValidValue(date))
                    throw Error.Argument("date",
                        "The string [" + date + "] is not a valid FHIR date string and isn't a FHIR datetime either");

                // This was a time, so we can just use the date portion of this
                date = new FhirDateTime(date).ToDateTimeOffset().Date.ToString("yyyy-MM-dd");
            }
            Value = date;
        }

        public string Value { get; }

        public override string ToString()
        {
            return Value;
        }

        public static DateValue Parse(string text)
        {
            return new DateValue(text);
        }
    }
}