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

        public SerilogLogger(SerilogSettings settings)
        {
            this._adaptee = new LoggerConfiguration()
                .MinimumLevel.Is(settings.MinimumLevel)
                .Enrich.WithProperty("Application", settings.ApplicationName)
                .Enrich.WithMachineName()
                .Enrich.FromLogContext()
                .WriteTo.RollingFile(settings.LogFile, outputTemplate: settings.OutputTemplate)
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


        public class SerilogSettings
        {
            public string ApplicationName { get; set; }
            public string LogPath { get; set; }
            public string LogFile { get; set; }
            public string OutputTemplate { get; set; }
            public LogEventLevel MinimumLevel { get; set; }

            public SerilogSettings()
            {
                this.ApplicationName = "DefaultApplicationName";
                this.LogPath = @"C:\logs";
                this.LogFile = String.Format(@"{0}\{1}-{{Date}}.log", this.LogPath, this.ApplicationName);
                this.OutputTemplate = @"{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} machine={MachineName} application={Application} level={Level} {Message}{NewLine}{Exception}";
                this.MinimumLevel = LogEventLevel.Information;
            }
        }
    }
}