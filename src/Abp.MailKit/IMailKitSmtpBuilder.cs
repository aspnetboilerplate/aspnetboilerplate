using System;
using MailKit.Net.Smtp;

namespace Abp.MailKit
{
    public interface IMailKitSmtpBuilder
    {
        SmtpClient Build();
    }
}