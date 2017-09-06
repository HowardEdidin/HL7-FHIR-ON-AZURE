#region Information

// Solution:  Spark
// Spark.Engine
// File:  IFhirResponseFactoryOld.cs
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

    public interface IFhirResponseFactoryOld
    {
        FhirResponse GetFhirResponse(Key key, IEnumerable<object> parameters = null);
        FhirResponse GetFhirResponse(Entry entry, IEnumerable<object> parameters = null);
        FhirResponse GetFhirResponse(Key key, params object[] parameters);
        FhirResponse GetFhirResponse(Entry entry, params object[] parameters);
    }
}