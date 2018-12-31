using Ratana.Library.Log;

namespace Ratana.Library.Profile
{
    public interface IProfiler
    {
        DisposableStopwatch GetStopwatch(LoggingEventType loggingEventType, string key);
    }
}
