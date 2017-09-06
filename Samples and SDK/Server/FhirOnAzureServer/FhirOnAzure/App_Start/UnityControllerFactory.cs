#region Information

// Solution:  Spark
// FhirOnAzure
// File:  UnityControllerFactory.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:40 PM

#endregion

namespace FhirOnAzure
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Microsoft.Practices.Unity;

    /// <summary>
    ///     Needed for injection in MVC controllers. DefaultControllerFactory does not take Unity into account.
    ///     (ApiControllers are also resolved correctly without this class.)
    ///     Based on this article:
    ///     http://www.codeproject.com/Articles/560798/ASP-NET-MVC-controller-dependency-injection-for-be
    /// </summary>
    public class UnityControllerFactory : DefaultControllerFactory
    {
        private readonly UnityContainer _container;

        public UnityControllerFactory(UnityContainer container)
        {
            _container = container;
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            IController result;
            try
            {
                result = (IController) _container.Resolve(controllerType);
            }
            catch (ResolutionFailedException)
            {
                //Doesn't matter, we'll leave it to the DefaultControllerFactory then.
                result = null;
            }

            return result ?? base.GetControllerInstance(requestContext, controllerType);
        }

        public override void ReleaseController(IController controller)
        {
            ((IDisposable) controller).Dispose();
        }
    }
}