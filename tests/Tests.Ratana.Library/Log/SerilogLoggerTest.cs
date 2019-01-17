using NUnit.Framework;
using Ratana.Library.Log;
using System;
using Tests.Ratana.Library.Attributes;

namespace Tests.Ratana.Library.Log
{
    [TestFixture]
    public class SerilogLoggerTest
    {
        private readonly SerilogLogger.SerilogSettings _serilogSettings = new SerilogLogger.SerilogSettings();

        [SetUp]
        public void Initialize()
        {
            this._serilogSettings.ApplicationName = "SerilogLoggerTest";
            this._serilogSettings.LogPath = @"C:\logs";
            this._serilogSettings.LogFile = $"{this._serilogSettings.LogPath}\\{this._serilogSettings.ApplicationName}-.log";
            this._serilogSettings.MinimumLevel = Serilog.Events.LogEventLevel.Verbose;
        }

        [Test]
        [Continuous, Integration]
        public void LogJustMessage()
        {
            #region Arrange
            // Set up some variables
            ILogger logger = new SerilogLogger(this._serilogSettings);
            #endregion

            #region Act
            // Log each level
            var message = "Testing SerilogLoggerTest.LogJustMessage()";
            logger.Verbose("    " + message);
            logger.Debug("      " + message);
            logger.Information(message);
            logger.Warning("    " + message);
            logger.Error("      " + message);
            logger.Fatal("      " + message);
            #endregion

            #region Assert
            // TODO check the actual log file
            // but for now just making sure there's no exception.
            #endregion
        }

        [Test]
        [Continuous, Integration]
        public void LogWithArguments()
        {
            #region Arrange
            // Set up some variables
            ILogger logger = new SerilogLogger(this._serilogSettings);
            #endregion

            #region Act
            // Log each level with arguments
            var message = "Testing SerilogLoggerTest.LogWithArguments() {str}, {num}, {object}, {loggerToString}, {@loggerSerialized}";
            logger.Verbose("    " + message, null, "one", 2, new { a = "aaa", b = "bbb" }, logger, logger);
            logger.Debug("      " + message, null, "one", 2, new { a = "aaa", b = "bbb" }, logger, logger);
            logger.Information(message, null, "one", 2, new { a = "aaa", b = "bbb" }, logger, logger);
            logger.Warning("    " + message, null, "one", 2, new { a = "aaa", b = "bbb" }, logger, logger);
            logger.Error("      " + message, null, "one", 2, new { a = "aaa", b = "bbb" }, logger, logger);
            logger.Fatal("      " + message, null, "one", 2, new { a = "aaa", b = "bbb" }, logger, logger);
            #endregion

            #region Assert
            // TODO check the actual log file
            // but for now just making sure there's no exception.
            #endregion
        }

        [Test]
        [Continuous, Integration]
        public void LogWithContext()
        {
            #region Arrange
            // Set up some variables
            ILogger logger = new SerilogLogger(this._serilogSettings);

            ILogContext context = new SerilogLogger.SerlogLogContext();
            context.Add("key1", "value1");
            context.Add("key2", "value2");
            context.Add("key3", "value3");
            #endregion
            
            #region Act
            // Log each level with context
            var message = "Testing SerilogLoggerTest.LogWithContext()";
            logger.Verbose(context, "    " + message);
            logger.Debug(context, "      " + message);
            logger.Information(context, message);
            logger.Warning(context, "    " + message);
            logger.Error(context, "      " + message);
            logger.Fatal(context, "      " + message);
            #endregion

            #region Assert
            // TODO check the actual log file
            // but for now just making sure there's no exception.
            #endregion
        }

        [Test]
        [Continuous, Integration]
        public void LogWithException()
        {
            #region Arrange
            // Set up some variables
            ILogger logger = new SerilogLogger(this._serilogSettings);
            #endregion

            #region Act
            // Log each level
            var ex = new Exception("Test Exception");

            // Log each level with Exceptions
            var message = "Testing SerilogLoggerTest.LogWithException()";
            logger.Verbose("    " + message, ex);
            logger.Debug("      " + message, ex);
            logger.Information(message, ex);
            logger.Warning("    " + message, ex);
            logger.Error("      " + message, ex);
            logger.Fatal("      " + message, ex);
            #endregion

            #region Assert
            // TODO check the actual log file
            // but for now just making sure there's no exception.
            #endregion
        }

        [Test]
        [Continuous, Integration]
        public void LogOnlyWarningOrAbove()
        {
            #region Arrange
            // override minimum level
            this._serilogSettings.MinimumLevel = Serilog.Events.LogEventLevel.Warning;

            // Set up some variables
            ILogger logger = new SerilogLogger(this._serilogSettings);
            #endregion
            
            #region Act
            // Log each level
            var ex = new Exception("Test Exception");

            // Log each level with Exceptions
            var message = "Testing SerilogLoggerTest.LogOnlyWarningOrAbove()";
            logger.Verbose("    " + message);
            logger.Debug("      " + message);
            logger.Information(message);
            logger.Warning("    " + message);
            logger.Error("      " + message);
            logger.Fatal("      " + message);
            #endregion
            
            #region Assert
            // TODO check the actual log file
            // but for now just making sure there's no exception.
            #endregion
        }
    }
}
