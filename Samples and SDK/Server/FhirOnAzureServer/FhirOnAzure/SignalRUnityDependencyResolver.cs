#region Information

// Solution:  Spark
// FhirOnAzure
// File:  SignalRUnityDependencyResolver.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:46 PM

#endregion

namespace FhirOnAzure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNet.SignalR;
    using Microsoft.Practices.Unity;

    public class SignalRUnityDependencyResolver : DefaultDependencyResolver
    {
        private readonly IUnityContainer _container;

        public SignalRUnityDependencyResolver(IUnityContainer container)
        {
            _container = container;
        }

        public override object GetService(Type serviceType)
        {
            try
            {
                return _container.Resolve(serviceType);
            }
            catch (Exception)
            {
                return base.GetService(serviceType);
            }
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            return _container.ResolveAll(serviceType).Concat(base.GetServices(serviceType));
        }
    }
}