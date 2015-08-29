using System;

namespace RatanaLibrary.Common.Log
{
    public static class LoggerExtentions
    {
        public static void Verbose(this ILogger logger, string message, string source = null, Exception exception = null)
        {
            logger.Log(new LogEntry(LoggingEventType.Verbose, message, source, exception));
        }

        public static void Debug(this ILogger logger, string message, string source = null, Exception exception = null)
        {
            logger.Log(new LogEntry(LoggingEventType.Debug, message, source, exception));
        }

        public static void Information(this ILogger logger, string message, string source = null, Exception exception = null)
        {
            logger.Log(new LogEntry(LoggingEventType.Information, message, source, exception));
        }

        public static void Warning(this ILogger logger, string message, string source = null, Exception exception = null)
        {
            logger.Log(new LogEntry(LoggingEventType.Warning, message, source, exception));
        }

        public static void Error(this ILogger logger, string message, string source = null, Exception exception = null)
        {
            logger.Log(new LogEntry(LoggingEventType.Error, message, source, exception));
        }

        public static void Fatal(this ILogger logger, string message, string source = null, Exception exception = null)
        {
            logger.Log(new LogEntry(LoggingEventType.Fatal, message, source, exception));
        }



        /// <summary>
        /// Log with specified severity.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="severity"></param>
        /// <param name="message"></param>
        /// <param name="source"></param>
        /// <param name="exception"></param>
        public static void Log(this ILogger logger, LoggingEventType severity, string message, string source = null, Exception exception = null)
        {
            logger.Log(new LogEntry(severity, message, source, exception));
        }
    }
}
