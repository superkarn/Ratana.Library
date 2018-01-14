using RatanaLibrary.Log;

namespace RatanaLibrary.Profile
{
    public class Profiler : IProfiler
    {
        private readonly ILogger _logger;

        public Profiler (ILogger logger)
        {
            this._logger = logger;
        }

        public DisposableStopwatch GetStopwatch(LoggingEventType loggingEventType, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                key = "Profiler.Stopwatch";
            }

            return new DisposableStopwatch(this._logger, loggingEventType, key);
        }
    }
}
