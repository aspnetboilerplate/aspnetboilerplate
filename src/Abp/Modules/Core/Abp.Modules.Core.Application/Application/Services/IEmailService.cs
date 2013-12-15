using System.Net.Mail;
using Abp.Application.Services;

namespace Abp.Modules.Core.Application.Services
{
    public interface IEmailService : IApplicationService
    {
        void SendEmail(MailMessage mail);
    }
}