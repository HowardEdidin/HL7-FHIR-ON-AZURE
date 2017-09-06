#region Information

// Solution:  Spark
// Spark.Engine
// File:  IPositionInfo.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:01 PM

#endregion

namespace FhirOnAzure.Search.Support
{
    public interface IPostitionInfo
    {
        int LineNumber { get; }
        int LinePosition { get; }
    }
}