using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Ratana.Library.Email;
using System;
using System.Net;
using System.Net.Mail;
using Tests.Ratana.Library.Attributes;

namespace Tests.Ratana.Library.Email
{
    [TestFixture]
    public class SystemNetEmailerTest
    {
        private SmtpClient _smptClient;

        [SetUp]
        public void Initialize()
        {
            // Get environment name
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            // Get Redis info from config
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true)
                .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                .Build();
            
            // Default Redis host:port is localhost:6379
            var host = string.IsNullOrWhiteSpace(config["smtp:host"]) ? "localhost" : config["smtp:host"];
            var username = string.IsNullOrWhiteSpace(config["smtp:username"]) ? "localhost" : config["smtp:username"];
            var password = string.IsNullOrWhiteSpace(config["smtp:password"]) ? "localhost" : config["smtp:password"];

            int port = 587;
            try { port = int.Parse(config["smtp:port"]); } catch { }

            bool enableSsl = true;
            try { enableSsl = bool.Parse(config["smtp:enableSsl"]); } catch { }

            this._smptClient = new SmtpClient()
            {
                Host = host,
                Port = port,
                EnableSsl = enableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(username, password)
            };
        }
        
        [Test]
        [Integration, RequireThirdPartyService]
        [TestCase("from@example.com", "to@example.com", "Ratana.Library.Common SystemNetEmailerTest", "")]
        [TestCase("from@example.com", "to@example.com", "Ratana.Library.Common SystemNetEmailerTest", "SendEmail_ShouldSendSuccessfully_WhenPassedAppropriateParameters")]
        public void SendEmail_ShouldSendSuccessfully_WhenPassedAppropriateParameters(string fromAddress, string toAddress, string subject, string body)
        {
            #region Arrange
            IEmailer emailer = new SystemNetEmailer(this._smptClient);
            using (var mailMessage = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                #region Act
                emailer.Send(mailMessage);
                #endregion
            }
            #endregion

            #region Assert
            // 1. Make sure there's no exceptions
            // 2. If using a valid toAddress, check that the message arrived
            #endregion
        }

        [Test]
        [Integration, RequireThirdPartyService]
        [TestCase("from@example.com", "to@example.com", "Ratana.Library.Common SystemNetEmailerTest", "")]
        [TestCase("from@example.com", "to@example.com", "Ratana.Library.Common SystemNetEmailerTest", "SendEmail_ShouldSendSuccessfully_WhenPassedAppropriateParameters")]
        public void SendEmail_ShouldSendSuccessfully_WhenCallingViaExtentionMethod(string fromAddress, string toAddress, string subject, string body)
        {
            #region Arrange
            IEmailer emailer = new SystemNetEmailer(this._smptClient);
            #endregion

            #region Act
            emailer.Send(fromAddress, toAddress, subject, body);
            #endregion

            #region Assert
            // 1. Make sure there's no exceptions
            // 2. If using a valid toAddress, check that the message arrived
            #endregion
        }
    }
}
