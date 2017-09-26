using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Net.Mail;
using Microsoft.AspNet.Identity;

namespace Abp.IdentityFramework
{
    public class IdentityEmailMessageService : IIdentityMessageService, ITransientDependency
    {
        private readonly IEmailSender _emailSender;

        public IdentityEmailMessageService(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public virtual Task SendAsync(IdentityMessage message)
        {
            return _emailSender.SendAsync(message.Destination, message.Subject, message.Body);
        }
    }
}
