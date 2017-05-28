using Serilog;
using Serilog.Events;
using System;
using System.Configuration;

namespace RatanaLibrary.Common.Log
{
    /// <summary>
    /// ILogger implementation using Serilog.
    /// Got the idea from https://stackoverflow.com/a/39499230/1398750
    /// </summary>
    public class SerilogLogger : ILogger
    {
        private readonly Serilog.ILogger _adaptee;

        public SerilogLogger(string applicationName = "DefaultApplicationName")
        {
            var logPath = ConfigurationManager.AppSettings["log:path"] ?? @"C:\logs";
            var logFile = String.Format(@"{0}\{1}-{{Date}}.log", logPath, applicationName);
            var outputTemplate = @"{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} machine={MachineName} application={Application} level={Level} {Message}{NewLine}{Exception}";

            // Try to get the minimun log level from config.  If failed, defaults to Information.
            LogEventLevel mininumLevel;
            if(Enum.TryParse(ConfigurationManager.AppSettings["log:minimum-level"], out mininumLevel) == false)
            {
                mininumLevel = LogEventLevel.Information;
            }

            this._adaptee = new LoggerConfiguration()
                .MinimumLevel.Is(mininumLevel)
                .Enrich.WithProperty("Application", applicationName)
                .Enrich.WithMachineName()
                .Enrich.FromLogContext()
                .WriteTo.RollingFile(logFile, outputTemplate: outputTemplate)
                .CreateLogger();
        }

        public void Log(LogEntry entry)
        {
            switch(entry.Severity)
            {
                case LoggingEventType.Verbose:
                    this._adaptee.Verbose(entry.Exception, entry.Message, entry.Arguments);
                    break;

                case LoggingEventType.Debug:
                    this._adaptee.Debug(entry.Exception, entry.Message, entry.Arguments);
                    break;

                case LoggingEventType.Information:
                    this._adaptee.Information(entry.Exception, entry.Message, entry.Arguments);
                    break;

                case LoggingEventType.Warning:
                    this._adaptee.Warning(entry.Exception, entry.Message, entry.Arguments);
                    break;

                case LoggingEventType.Error:
                    this._adaptee.Error(entry.Exception, entry.Message, entry.Arguments);
                    break;

                case LoggingEventType.Fatal:
                default:
                    this._adaptee.Fatal(entry.Exception, entry.Message, entry.Arguments);
                    break;
            }
        }
    }
}