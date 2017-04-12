using System;
using MailKit.Net.Smtp;

namespace Abp.MailKit
{
    public interface IAbpMailKitConfiguration
    {
        Action<SmtpClient> SmtpClientConfigurer { get; set; }
    }
}
