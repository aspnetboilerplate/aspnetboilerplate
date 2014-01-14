using Abp.Modules.Core.Domain.Services;
using Taskever.Application.Services;

namespace Taskever.Domain.Services.Impl
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