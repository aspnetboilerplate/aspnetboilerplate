using System;
using Abp.Application.Services.Dto;

namespace Abp.Notifications
{
    public class UserNotification : EntityDto<Guid>
    {
        public long UserId { get; set; }

        public UserNotificationState State { get; set; }

        public Notification Notification { get; set; }
    }
}