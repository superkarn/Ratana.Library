using RatanaLibrary.Log;

namespace RatanaLibrary.Profile
{
    public interface IProfiler
    {
        DisposableStopwatch GetStopwatch(LoggingEventType loggingEventType, string key);
    }
}
