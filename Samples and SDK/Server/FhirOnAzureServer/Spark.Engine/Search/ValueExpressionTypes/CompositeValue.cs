#region Information

// Solution:  Spark
// Spark.Engine
// File:  CompositeValue.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:01 PM

#endregion

namespace FhirOnAzure.Search
{
    using System.Collections.Generic;
    using System.Linq;
    using Support;

    public class CompositeValue : ValueExpression
    {
        private const char TUPLESEPARATOR = '$';

        public CompositeValue(ValueExpression[] components)
        {
            if (components == null) throw Error.ArgumentNull("components");

            Components = components;
        }

        public CompositeValue(IEnumerable<ValueExpression> components)
        {
            if (components == null) throw Error.ArgumentNull("components");

            Components = components.ToArray();
        }

        public ValueExpression[] Components { get; }

        public override string ToString()
        {
            var values = Components.Select(v => v.ToString());
            return string.Join(TUPLESEPARATOR.ToString(), values);
        }


        public static CompositeValue Parse(string text)
        {
            if (text == null) throw Error.ArgumentNull("text");

            var values = text.SplitNotEscaped(TUPLESEPARATOR);

            return new CompositeValue(values.Select(v => new UntypedValue(v)));
        }
    }
}