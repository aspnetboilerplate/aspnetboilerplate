namespace Abp.Net.Mail.Smtp
{
    public interface ISmtpClientProviderConfiguration
    {
        string Host { get; }

        int Port { get; }

        string UserName { get; }

        string Password { get; }

        string Domain { get; }

        bool EnableSsl { get; }

        bool UseDefaultCredentials { get; }
    }
}