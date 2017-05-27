using Serilog;
using System;

namespace RatanaLibrary.Common.Log
{
    /// <summary>
    /// ILogger implementation using Serilog.
    /// Got the idea from https://stackoverflow.com/a/39499230/1398750
    /// </summary>
    public class SerilogLogger : ILogger
    {
        private readonly Serilog.ILogger _adaptee;

        public SerilogLogger()
        {
            this._adaptee = new LoggerConfiguration()
                .WriteTo.RollingFile(@"C:\logs\log.txt")
                .CreateLogger();
        }

        public SerilogLogger(Serilog.ILogger adaptee)
        {
            this._adaptee = adaptee;
        }

        public void Log(LogEntry entry)
        {
            switch(entry.Severity)
            {
                case LoggingEventType.Verbose:
                    this._adaptee.Verbose(entry.Exception, entry.Message);
                    break;

                case LoggingEventType.Debug:
                    this._adaptee.Debug(entry.Exception, entry.Message);
                    break;

                case LoggingEventType.Information:
                    this._adaptee.Information(entry.Exception, entry.Message);
                    break;

                case LoggingEventType.Warning:
                    this._adaptee.Warning(entry.Exception, entry.Message);
                    break;

                case LoggingEventType.Error:
                    this._adaptee.Error(entry.Exception, entry.Message);
                    break;

                case LoggingEventType.Fatal:
                default:
                    this._adaptee.Fatal(entry.Exception, entry.Message);
                    break;
            }
        }
    }
}