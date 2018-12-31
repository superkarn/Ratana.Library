using System;

namespace Ratana.Library.Log
{
    public class LogEntry
    {
        public readonly LoggingEventType Severity;
        public readonly string Message;
        public readonly Exception Exception;
        public readonly object[] Arguments;

        public LogEntry(LoggingEventType severity, string message, Exception exception, object[] args)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException("message");
            }

            if (severity < LoggingEventType.Verbose || LoggingEventType.Fatal < severity)
            {
                throw new ArgumentOutOfRangeException("severity");
            }

            this.Severity = severity;
            this.Message = message;
            this.Exception = exception;
            this.Arguments = args;
        }
    }
}
