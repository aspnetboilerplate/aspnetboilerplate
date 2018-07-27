using MailKit.Security;

namespace Abp.MailKit
{
    public class AbpMailKitConfiguration : IAbpMailKitConfiguration
    {
        public SecureSocketOptions? SecureSocketOption { get; set; }
    }
}
