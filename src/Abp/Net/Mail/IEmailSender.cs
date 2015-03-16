using System.Net.Mail;
using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.Net.Mail
{
    public interface IEmailSender
    {
        Task SendAsync(string to, string subject, string body);

        Task SendAsync(MailMessage mail);
    }
}
