using Moq;
using NUnit.Framework;
using RatanaLibrary.Email;
using System.Net;
using System.Net.Mail;
using Tests.RatanaLibrary.Attributes;

namespace Tests.RatanaLibrary.Email
{
    [TestFixture]
    public class SystemNetEmailerTest
    {
        private SmtpClient _smptClient;

        // fill this out when you want to run the tests
        private readonly string _smtpHost = "";
        private readonly int _smtpPort = 587;
        private readonly string _smtpUsername = "";
        private readonly string _smtpPassword = "";
        

        [Test]
        [Integration]
        [TestCase("to@example.com", "RatanaLibrary.Common SystemNetEmailerTest", "")]
        [TestCase("to@example.com", "RatanaLibrary.Common SystemNetEmailerTest", "SendEmail_ShouldSendSuccessfully_WhenPassedAppropriateParameters")]
        public void SendEmail_ShouldSendSuccessfully_WhenPassedAppropriateParameters( string toAddress, string subject, string body)
        {
            #region Arrange
            _smptClient = new SmtpClient()
            {
                Host = _smtpHost,
                Port = _smtpPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_smtpUsername, _smtpPassword)
            };

            IEmailer emailer = new SystemNetEmailer(_smptClient);
            using (var mailMessage = new MailMessage(_smtpUsername, toAddress)
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
        [Integration]
        [TestCase("from@example.com", "to@example.com", "RatanaLibrary.Common SystemNetEmailerTest", "")]
        [TestCase("from@example.com", "to@example.com", "RatanaLibrary.Common SystemNetEmailerTest", "SendEmail_ShouldSendSuccessfully_WhenPassedAppropriateParameters")]
        public void SendEmail_ShouldSendSuccessfully_WhenCallingViaExtentionMethod(string fromAddress, string toAddress, string subject, string body)
        {
            #region Arrange
            _smptClient = new SmtpClient()
            {
                Host = _smtpHost,
                Port = _smtpPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_smtpUsername, _smtpPassword)
            };

            IEmailer emailer = new SystemNetEmailer(_smptClient);
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
