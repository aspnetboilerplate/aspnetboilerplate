using MailKit.Security;

namespace Abp.MailKit
{
    public interface IAbpMailKitConfiguration
    {
        SecureSocketOptions? SecureSocketOption { get; set; }
    }
}