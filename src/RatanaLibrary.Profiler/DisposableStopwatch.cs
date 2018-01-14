using RatanaLibrary.Log;
using System;
using System.Diagnostics;

namespace RatanaLibrary.Profile
{
    public interface IDisposableStopwatch : IDisposable
    {

    }

    public class DisposableStopwatch : IDisposableStopwatch
    {
        private readonly LoggingEventType _severity;
        private readonly ILogger _logger;
        private readonly Stopwatch _stopwatch;
        private readonly string _key;

        public DisposableStopwatch(ILogger logger, LoggingEventType severity, string key = "")
        {
            this._logger = logger;
            this._severity = severity;
            this._stopwatch = Stopwatch.StartNew();
            this._key = key;
        }

        public void Dispose()
        {
            if (this._stopwatch != null)
            {
                this._stopwatch.Stop();

                // TODO log something better
                this._logger.Log(this._severity, $"key={this._key} elapsedTime={this._stopwatch.Elapsed.ToString()}");
            }
        }
    }
}
