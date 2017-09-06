#region Information

// Solution:  Spark
// Spark.Engine
// File:  ChoiceValue.cs
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

    public class ChoiceValue : ValueExpression
    {
        private const char VALUESEPARATOR = ',';

        public ChoiceValue(ValueExpression[] choices)
        {
            if (choices == null) Error.ArgumentNull("choices");

            Choices = choices;
        }

        public ChoiceValue(IEnumerable<ValueExpression> choices)
        {
            if (choices == null) Error.ArgumentNull("choices");

            Choices = choices.ToArray();
        }

        public ValueExpression[] Choices { get; }

        public override string ToString()
        {
            var values = Choices.Select(v => v.ToString());
            return string.Join(VALUESEPARATOR.ToString(), values);
        }

        public static ChoiceValue Parse(string text)
        {
            if (text == null) Error.ArgumentNull("text");

            var values = text.SplitNotEscaped(VALUESEPARATOR);

            return new ChoiceValue(values.Select(v => splitIntoComposite(v)));
        }

        private static ValueExpression splitIntoComposite(string text)
        {
            var composite = CompositeValue.Parse(text);

            // If there's only one component, this really was a single value
            if (composite.Components.Length == 1)
                return composite.Components[0];
            return composite;
        }
    }
}