using System.Net.Mail;

namespace RatanaLibrary.Common.Email
{
    public static class EmailerExtensions
    {
        public static void Send(this IEmailer emailer, string fromAddress, string toAddress, string subject, string body)
        {
            var mailMessage = new MailMessage(fromAddress, toAddress, subject, body);
            emailer.Send(mailMessage);
        }
    }
}
