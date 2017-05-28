using NUnit.Framework;
using RatanaLibrary.Common.Log;
using System;

namespace Tests.RatanaLibrary.Common.Log
{

    [TestFixture]
    public class SerilogLoggerTest
    {
        [Test]
        public void NoException()
        {
            #region Arrange
            // Set up some variables
            var logger = new SerilogLogger(applicationName: "SerilogLoggerTest");

            // TODO set minimum level to verbose, so we can test all levels
            #endregion


            #region Act
            // Log each level
            logger.Verbose("Test Verbose log.");
            logger.Debug("Test Debug log.");
            logger.Information("Test Information log.");
            logger.Warning("Test Warning log.");
            logger.Error("Test Error log.");
            logger.Fatal("Test Fatal log.");

            var source = "Test Source";
            var ex = new Exception("Test Exception");

            // Log each level with Exceptions
            logger.Verbose("Test Verbose log.", source, ex);
            logger.Debug("Test Debug log.", source, ex);
            logger.Information("Test Information log.", source, ex);
            logger.Warning("Test Warning log.", source, ex);
            logger.Error("Test Error log.", source, ex);
            logger.Fatal("Test Fatal log.", source, ex);
            #endregion


            #region Assert
            // Nothing to do.  Just making sure there's no exception.
            #endregion
        }
    }
}
