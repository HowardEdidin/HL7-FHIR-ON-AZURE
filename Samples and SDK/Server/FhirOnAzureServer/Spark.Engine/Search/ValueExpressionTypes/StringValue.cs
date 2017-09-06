#region Information

// Solution:  Spark
// Spark.Engine
// File:  StringValue.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:01 PM

#endregion

namespace FhirOnAzure.Search
{
    public class StringValue : ValueExpression
    {
        public StringValue(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public override string ToString()
        {
            return EscapeString(Value);
        }

        public static StringValue Parse(string text)
        {
            return new StringValue(UnescapeString(text));
        }


        internal static string EscapeString(string value)
        {
            if (value == null) return null;

            value = value.Replace(@"\", @"\\");
            value = value.Replace(@"$", @"\$");
            value = value.Replace(@",", @"\,");
            value = value.Replace(@"|", @"\|");

            return value;
        }

        internal static string UnescapeString(string value)
        {
            if (value == null) return null;

            value = value.Replace(@"\|", @"|");
            value = value.Replace(@"\,", @",");
            value = value.Replace(@"\$", @"$");
            value = value.Replace(@"\\", @"\");

            return value;
        }
    }
}