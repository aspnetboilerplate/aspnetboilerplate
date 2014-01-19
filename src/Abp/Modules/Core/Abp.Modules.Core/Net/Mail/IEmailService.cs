using System.Net.Mail;
using Abp.Domain.Services;

namespace Abp.Net.Mail
{
    public interface IEmailService : IDomainService
    {
        void SendEmail(MailMessage mail);
    }
}