using System.Net.Mail;

namespace Taskever.Notifications
{
    public interface INotification
    {
        MailMessage CreateMailMessage();
    }
}