#region Information

// Solution:  Spark
// Spark.Engine
// File:  SparkEngineEventSource.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:02 PM

#endregion

namespace FhirOnAzure.Engine.Logging
{
    using System;
    using System.Diagnostics.Tracing;

    [EventSource(Name = "Furore-FhirOnAzure-Engine")]
    public sealed class SparkEngineEventSource : EventSource
    {
        private static readonly Lazy<SparkEngineEventSource> Instance =
            new Lazy<SparkEngineEventSource>(() => new SparkEngineEventSource());

        private SparkEngineEventSource()
        {
        }

        public static SparkEngineEventSource Log => Instance.Value;

        [Event(1, Message = "Service call: {0}",
            Level = EventLevel.Verbose, Keywords = Keywords.ServiceMethod)]
        internal void ServiceMethodCalled(string methodName)
        {
            WriteEvent(1, methodName);
        }

        [Event(2, Message = "Not supported: {0} in {1}",
            Level = EventLevel.Verbose, Keywords = Keywords.Unsupported)]
        internal void UnsupportedFeature(string methodName, string feature)
        {
            WriteEvent(2, feature, methodName);
        }

        [Event(4, Message = "Invalid Element",
            Level = EventLevel.Verbose, Keywords = Keywords.Unsupported)]
        internal void InvalidElement(string resourceId, string element, string message)
        {
            WriteEvent(4, message, resourceId, element);
        }

        public class Keywords
        {
            public const EventKeywords ServiceMethod = (EventKeywords) 1;
            public const EventKeywords Invalid = (EventKeywords) 2;
            public const EventKeywords Unsupported = (EventKeywords) 4;
            public const EventKeywords Tracing = (EventKeywords) 8;
        }

        public class Tasks
        {
            public const EventTask ServiceMethod = (EventTask) 1;
        }
    }
}