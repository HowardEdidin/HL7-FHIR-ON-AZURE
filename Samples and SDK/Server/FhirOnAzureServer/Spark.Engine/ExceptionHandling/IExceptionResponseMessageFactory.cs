#region Information

// Solution:  Spark
// Spark.Engine
// File:  IExceptionResponseMessageFactory.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:51 PM

#endregion

namespace FhirOnAzure.Engine.ExceptionHandling
{
    using System;
    using System.Net.Http;

    public interface IExceptionResponseMessageFactory
    {
        HttpResponseMessage GetResponseMessage(Exception exception, HttpRequestMessage reques);
    }
}