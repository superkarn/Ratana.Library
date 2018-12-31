using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ratana.Library.Log
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
                .Enrich.FromLogContext()
                .WriteTo.File(settings.LogFile, outputTemplate:settings.OutputTemplate, rollingInterval:RollingInterval.Day)
                .CreateLogger();
        }

        public void Log(ILogContext context, LogEntry entry)
        {
            var message = entry.Message;

            // If log context is available, prepend it to the beginning of the message.
            // Not using Serilog LogContext because it requires using() for each property
            // For now, format it ourselves.
            if (context != null)
            {
                message = $"{context.ToString()} {entry.Message}";
            }

            switch (entry.Severity)
            {
                case LoggingEventType.Verbose:
                    this._adaptee.Verbose(entry.Exception, message, entry.Arguments);
                    break;

                case LoggingEventType.Debug:
                    this._adaptee.Debug(entry.Exception, message, entry.Arguments);
                    break;

                case LoggingEventType.Information:
                    this._adaptee.Information(entry.Exception, message, entry.Arguments);
                    break;

                case LoggingEventType.Warning:
                    this._adaptee.Warning(entry.Exception, message, entry.Arguments);
                    break;

                case LoggingEventType.Error:
                    this._adaptee.Error(entry.Exception, message, entry.Arguments);
                    break;

                case LoggingEventType.Fatal:
                default:
                    this._adaptee.Fatal(entry.Exception, message, entry.Arguments);
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
                this.LogFile = $"{this.LogPath}\\{this.ApplicationName}-.log";
                this.OutputTemplate = @"{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} application={Application} level={Level} {Message}{NewLine}{Exception}";
                this.MinimumLevel = LogEventLevel.Information;
            }
        }


        public class SerlogLogContext : ILogContext
        {
            public IDictionary<string, string> Data = new Dictionary<string, string>();

            void ILogContext.Add(string key, string value)
            {
                this.Data.Add(key, value);
            }

            bool ILogContext.ContainsKey(string key)
            {
                return this.Data.ContainsKey(key);
            }

            bool ILogContext.Remove(string key)
            {
                return this.Data.Remove(key);
            }

            bool ILogContext.TryGetValue(string key, out string value)
            {
                return this.Data.TryGetValue(key, out value);
            }
            public override string ToString()
            {
                return string.Join(" ", this.Data.Select(x => $"{x.Key}={x.Value}").ToArray());
            }
        }
    }
}