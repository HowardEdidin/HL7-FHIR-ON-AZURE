namespace FhirOnAzure.Mongo
{
    using System;
    using System.Diagnostics.Tracing;

    [EventSource(Name = "Furore-FhirOnAzure-Mongo")]
    public sealed class SparkMongoEventSource : EventSource
    {
        //public class Tasks
        //{
        //    public const EventTask ServiceMethod = (EventTask)1;
        //}

        private static readonly Lazy<SparkMongoEventSource> Instance =
            new Lazy<SparkMongoEventSource>(() => new SparkMongoEventSource());

        private SparkMongoEventSource()
        {
        }

        public static SparkMongoEventSource Log => Instance.Value;

        [Event(1, Message = "Method call: {0}",
            Level = EventLevel.Verbose, Keywords = Keywords.Tracing)]
        internal void ServiceMethodCalled(string methodName)
        {
            WriteEvent(1, methodName);
        }

        public class Keywords
        {
            public const EventKeywords Tracing = (EventKeywords) 1;
            public const EventKeywords Unsupported = (EventKeywords) 2;
        }
    }
}