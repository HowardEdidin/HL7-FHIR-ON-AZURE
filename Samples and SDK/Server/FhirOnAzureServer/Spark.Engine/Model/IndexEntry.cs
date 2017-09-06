#region Information

// Solution:  Spark
// Spark.Engine
// File:  IndexEntry.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:01 PM

#endregion

namespace FhirOnAzure.Engine.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using FhirOnAzure.Search;

    public class IndexValue : ValueExpression
    {
        private readonly List<Expression> _values;

        public IndexValue()
        {
            _values = new List<Expression>();
        }

        public IndexValue(string name) : this()
        {
            Name = name;
        }

        public IndexValue(string name, List<Expression> values) : this(name)
        {
            Values = values;
        }

        public IndexValue(string name, params Expression[] values) : this(name)
        {
            Values = values.ToList();
        }

        public string Name { get; set; }

        public List<Expression> Values
        {
            get => _values;
            set => _values.AddRange(value);
        }

        public void AddValue(Expression value)
        {
            _values.Add(value);
        }
    }

    public static class IndexValueExtensions
    {
        public static IEnumerable<IndexValue> IndexValues(this IndexValue root)
        {
            return root.Values.OfType<IndexValue>();
        }
    }
}