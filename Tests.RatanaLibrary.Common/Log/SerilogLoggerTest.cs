using NUnit.Framework;
using RatanaLibrary.Common.Log;

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
            var logger = new SerilogLogger();

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
            #endregion


            #region Assert
            // Nothing to do.  Just making sure there's no exception.
            #endregion
        }
    }
}
