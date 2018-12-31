using System.Net.Mail;

namespace Ratana.Library.Email
{
    public interface IEmailer
    {
        void Send(MailMessage mailMessage);
    }
}
