#region Information

// Solution:  Spark
// Spark.Engine
// File:  IFhirResponseInterceptorRunner.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:03 PM

#endregion

namespace FhirOnAzure.Engine.Interfaces
{
    using System.Collections.Generic;
    using Core;

    public interface IFhirResponseInterceptorRunner
    {
        void AddInterceptor(IFhirResponseInterceptor interceptor);
        void ClearInterceptors();
        FhirResponse RunInterceptors(Entry entry, IEnumerable<object> parameters);
    }
}