#region Information

// Solution:  Spark
// Spark.Engine
// File:  CriteriumBuilder.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:01 PM

#endregion

namespace FhirOnAzure.Search
{
    using System;

    public interface IOperationBuilder
    {
        ICriteriumBuilder IsMissing { get; }
        ICriteriumBuilder Eq(decimal number);
        ICriteriumBuilder LessThan();

        IStringModifier Matches(string text);


        ITokenModifier Is(string code);

        IValueBuilder On(string dateTime);
        IValueBuilder On(DateTimeOffset dateTime);
        IValueBuilder Before();
        IValueBuilder After();

        ICriteriumBuilder References(string resource, string id);
        ICriteriumBuilder References(Uri location);
        ICriteriumBuilder References(string location);
    }

    public interface IReferenceBuilder
    {
    }

    public interface IValueBuilder
    {
    }

    public interface ICriteriumBuilder
    {
        IOperationBuilder And(string paramName);
    }

    public interface ITokenModifier : ICriteriumBuilder
    {
        ICriteriumBuilder In(string ns);
        ICriteriumBuilder In(Uri ns);
    }

    public interface IStringModifier : ICriteriumBuilder
    {
        ICriteriumBuilder Exactly { get; }
    }
}