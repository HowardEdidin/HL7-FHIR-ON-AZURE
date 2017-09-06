#region Information

// Solution:  Spark
// Spark.Engine
// File:  DependencyCoupler.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:50 PM

#endregion

namespace FhirOnAzure.Engine.Auxiliary
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using FhirOnAzure.Core;

    // Intermediate solution. Eventually replace with real resolver.

    public delegate object Instantiator();

    public static class DependencyCoupler
    {
        private static readonly Dictionary<Type, object> instances = new Dictionary<Type, object>();
        private static readonly Dictionary<Type, Instantiator> instanciators = new Dictionary<Type, Instantiator>();
        private static readonly Dictionary<Type, Type> types = new Dictionary<Type, Type>();

        private static volatile object access = new object();

        private static bool registered;

        public static void Register<I>(Instantiator instanciator)
        {
            instanciators.Add(typeof(I), instanciator);
        }

        public static void Register<I, T>()
        {
            types.Add(typeof(I), typeof(T));
        }

        public static void Register<I>(object instance)
        {
            instances.Add(typeof(I), instance);
        }

        public static T Inject<T>()
        {
            var instance = default(T);
            var key = typeof(T);
            Type type = null;
            Instantiator instanciator = null;


            object value;
            if (instances.TryGetValue(key, out value))
                instance = (T) value;
            else if (instanciators.TryGetValue(key, out instanciator))
                instance = (T) instanciator();
            else if (types.TryGetValue(key, out type))
                instance = (T) Activator.CreateInstance(type);
            else
                throw Error.Create(HttpStatusCode.InternalServerError,
                    "Dependency injection error: The type ({0}) you try to instanciate is not registered", key.Name);

            return instance;
        }

        public static void Configure(Action configure)
        {
            lock (access)
            {
                if (!registered)
                {
                    registered = true;
                    configure();
                }
            }
        }
    }
}