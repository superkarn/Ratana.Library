using RatanaLibrary.Common.Log;

namespace RatanaLibrary.Common.Profile
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
            return new DisposableStopwatch(this._logger, loggingEventType, key);
        }
    }
}
