#region Information

// Solution:  Spark
// Spark.Engine
// File:  Localhost.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:12 PM

#endregion

namespace FhirOnAzure.Engine.Core
{
    using System;

    public interface ILocalhost
    {
        Uri DefaultBase { get; }
        Uri Absolute(Uri uri);
        bool IsBaseOf(Uri uri);
        Uri GetBaseOf(Uri uri);
    }
}