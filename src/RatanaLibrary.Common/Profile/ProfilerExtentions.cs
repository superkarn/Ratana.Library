using RatanaLibrary.Common.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatanaLibrary.Common.Profile
{
    public static class ProfilerExtentions
    {
        /// <summary>
        /// Usage
        /// <code>
        /// using (profiler.GetStopwatchDebug("mykey")) { }
        /// </code>
        /// </summary>
        /// <param name="profiler"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static DisposableStopwatch GetStopwatchVerbose(this IProfiler profiler, string key)
        {
            return profiler.GetStopwatch(LoggingEventType.Verbose, key);
        }

        public static DisposableStopwatch GetStopwatchDebug(this IProfiler profiler, string key)
        {
            return profiler.GetStopwatch(LoggingEventType.Debug, key);
        }

        public static DisposableStopwatch GetStopwatchInformation(this IProfiler profiler, string key)
        {
            return profiler.GetStopwatch(LoggingEventType.Information, key);
        }
    }
}
