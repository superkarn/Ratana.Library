using System.Net.Mail;

namespace Ratana.Library.Email
{
    public static class EmailerExtensions
    {
        public static void Send(this IEmailer emailer, string fromAddress, string toAddress, string subject, string body)
        {
            using (var mailMessage = new MailMessage(fromAddress, toAddress, subject, body))
            {
                emailer.Send(mailMessage);
            }
        }
    }
}
