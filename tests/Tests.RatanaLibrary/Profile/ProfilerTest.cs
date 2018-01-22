using Moq;
using NUnit.Framework;
using RatanaLibrary.Log;
using RatanaLibrary.Profile;
using Tests.RatanaLibrary.Attributes;

namespace Tests.RatanaLibrary.Profile
{
    [TestFixture]
    public class ProfilerTest
    {
        [Test]
        [Continuous, Integration]
        [TestCase("")]
        [TestCase("MyKey")]
        [TestCase("My-Key")]
        [TestCase("My:Key")]
        public void DisposableStopwatch_ShouldNotThrowException(string key)
        {
            // Arrange
            //SerilogLogger.SerilogSettings settings = new SerilogLogger.SerilogSettings();
            //settings.ApplicationName = "SerilogLoggerTest";
            //settings.LogPath = @"C:\logs";
            //settings.LogFile = string.Format(@"{0}\{1}-{{Date}}.log", settings.LogPath, settings.ApplicationName);
            //ILogger logger = new SerilogLogger(settings);

            var logger = new Mock<ILogger>();

            IProfiler profiler = new Profiler(logger.Object);

            // Action
            using (profiler.GetStopwatchInformation(key))
            {
                System.Threading.Thread.Sleep(1);
            }

            // Assert there should not be any exception
        }
    }
}
