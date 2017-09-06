using System;
using System.Diagnostics;

namespace FhirOnAzure.MetaStore
{
    internal static class Performance
    {
        public static int Measure(Action action)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            action();

            stopwatch.Stop();
            return stopwatch.Elapsed.Seconds;

        }
    }
}