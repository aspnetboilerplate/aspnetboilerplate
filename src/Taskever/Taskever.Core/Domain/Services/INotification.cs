using System.Net.Mail;

namespace Taskever.Application.Services
{
    public interface INotification
    {
        MailMessage CreateMailMessage();
    }
}