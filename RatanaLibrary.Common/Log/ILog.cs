
namespace RatanaLibrary.Common.Log
{
    /// <summary>
    /// Provides an easy-to-use generic logging interface.
    /// Got the idea from http://stackoverflow.com/a/5646876
    /// </summary>
    public interface ILogger
    {
        void Log(LogEntry entry);
    }
}
