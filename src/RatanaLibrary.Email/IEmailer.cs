using System.Net.Mail;

namespace RatanaLibrary.Email
{
    public interface IEmailer
    {
        void Send(MailMessage mailMessage);
    }
}
