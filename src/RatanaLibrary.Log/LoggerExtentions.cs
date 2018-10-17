using System;

namespace RatanaLibrary.Log
{
    public static class LoggerExtentions
    {
        public static void Verbose(this ILogger logger, string message, Exception exception = null, params object[] args)
        {
            logger.Verbose(null, message, exception, args);
        }
        public static void Verbose(this ILogger logger, ILogContext context, string message, Exception exception = null, params object[] args)
        {
            logger.Log(context, new LogEntry(LoggingEventType.Verbose, message, exception, args));
        }

        public static void Debug(this ILogger logger, string message, Exception exception = null, params object[] args)
        {
            logger.Log(null, new LogEntry(LoggingEventType.Debug, message, exception, args));
        }
        public static void Debug(this ILogger logger, ILogContext context, string message, Exception exception = null, params object[] args)
        {
            logger.Log(context, new LogEntry(LoggingEventType.Debug, message, exception, args));
        }

        public static void Information(this ILogger logger, string message, Exception exception = null, params object[] args)
        {
            logger.Log(null, new LogEntry(LoggingEventType.Information, message, exception, args));
        }
        public static void Information(this ILogger logger, ILogContext context, string message, Exception exception = null, params object[] args)
        {
            logger.Log(context, new LogEntry(LoggingEventType.Information, message, exception, args));
        }

        public static void Warning(this ILogger logger, string message, Exception exception = null, params object[] args)
        {
            logger.Log(null, new LogEntry(LoggingEventType.Warning, message, exception, args));
        }
        public static void Warning(this ILogger logger, ILogContext context, string message, Exception exception = null, params object[] args)
        {
            logger.Log(context, new LogEntry(LoggingEventType.Warning, message, exception, args));
        }

        public static void Error(this ILogger logger, string message, Exception exception = null, params object[] args)
        {
            logger.Log(null, new LogEntry(LoggingEventType.Error, message, exception, args));
        }
        public static void Error(this ILogger logger, ILogContext context, string message, Exception exception = null, params object[] args)
        {
            logger.Log(context, new LogEntry(LoggingEventType.Error, message, exception, args));
        }

        public static void Fatal(this ILogger logger, string message, Exception exception = null, params object[] args)
        {
            logger.Log(null, new LogEntry(LoggingEventType.Fatal, message, exception, args));
        }
        public static void Fatal(this ILogger logger, ILogContext context, string message, Exception exception = null, params object[] args)
        {
            logger.Log(context, new LogEntry(LoggingEventType.Fatal, message, exception, args));
        }



        /// <summary>
        /// Log with specified severity.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="severity"></param>
        /// <param name="message"></param>
        /// <param name="source"></param>
        /// <param name="exception"></param>
        public static void Log(this ILogger logger, LoggingEventType severity, string message, Exception exception = null, params object[] args)
        {
            logger.Log(null, new LogEntry(severity, message, exception, args));
        }
    }
}
