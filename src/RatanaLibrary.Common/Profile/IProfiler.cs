using RatanaLibrary.Common.Log;

namespace RatanaLibrary.Common.Profile
{
    public interface IProfiler
    {
        DisposableStopwatch GetStopwatch(LoggingEventType loggingEventType, string key);
    }
}
