namespace Abp.Net.Mail.Smtp
{
    public interface ISmtpEmailSenderConfiguration
    {
        string DefaultFromAddress { get; }
        
        string DefaultFromDisplayName { get; }
    }
}