using Abp.Modules.Core.Domain.Services;

namespace Taskever.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly IEmailService _emailService;

        public NotificationService(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public void Notify(INotification notification)
        {
            var mail = notification.CreateMailMessage();
            _emailService.SendEmail(mail);
        }
    }
}