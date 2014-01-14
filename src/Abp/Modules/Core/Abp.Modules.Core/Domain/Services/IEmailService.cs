using System.Net.Mail;
using Abp.Domain.Services;

namespace Abp.Modules.Core.Domain.Services
{
    public interface IEmailService : IDomainService
    {
        void SendEmail(MailMessage mail);
    }
}