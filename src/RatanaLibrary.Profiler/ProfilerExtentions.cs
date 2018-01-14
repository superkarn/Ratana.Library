using RatanaLibrary.Log;

namespace RatanaLibrary.Profile
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
