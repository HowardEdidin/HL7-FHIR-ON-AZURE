#region Information

// Solution:  Spark
// Spark.Engine
// File:  ExtendableWith.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:52 PM

#endregion

namespace FhirOnAzure.Engine.Storage
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Store.Interfaces;

    public class ExtendableWith<T> : IExtendableWith<T>, IEnumerable<T>
    {
        private readonly Dictionary<Type, T> extensions;

        public ExtendableWith()
        {
            extensions = new Dictionary<Type, T>();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return extensions.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AddExtension<TV>(TV extension) where TV : T
        {
            foreach (var interfaceType in extension.GetType().GetInterfaces().Where(i => typeof(T).IsAssignableFrom(i)))
                extensions[interfaceType] = extension;
        }

        public void RemoveExtension<TV>() where TV : T
        {
            extensions.Remove(typeof(TV));
        }

        public TV FindExtension<TV>() where TV : T
        {
            if (extensions.ContainsKey(typeof(TV)))
                return (TV) extensions[typeof(TV)];
            return default(TV);
        }

        public void RemoveExtension(Type type)
        {
            extensions.Remove(type);
        }

        public T FindExtension(Type type)
        {
            var key = extensions.Keys.SingleOrDefault(k => type.IsAssignableFrom(k));
            if (key != null)
                return extensions[key];

            return default(T);
        }
    }
}