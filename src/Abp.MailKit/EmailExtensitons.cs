#if NET46
using MimeKit;
using System.Net.Mail;


namespace Abp.MailKit
{
    public static class EmailExtensitons
    {
        public static MimeMessage ToMimeMessage(this MailMessage mail)
        {
            var message = new MimeMessage();
            var bodyType = mail.IsBodyHtml ? "html" : "plain";

            message.From.Add(new MailboxAddress(mail.From.Address));

            foreach (var to in mail.To)
            {
                message.To.Add(new MailboxAddress(to.Address));
            }

            message.Subject = mail.Subject;
            message.Body = new TextPart(bodyType)
            {
                Text = mail.Body
            };

            return message;
        }
    }
}
#endif
