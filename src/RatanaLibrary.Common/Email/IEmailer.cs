using System.Net.Mail;

namespace RatanaLibrary.Common.Email
{
    public interface IEmailer
    {
        void Send(MailMessage mailMessage);
    }
}
