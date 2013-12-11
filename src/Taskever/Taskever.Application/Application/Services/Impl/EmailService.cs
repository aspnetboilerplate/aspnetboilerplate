using System.Net.Mail;

namespace Taskever.Application.Services.Impl
{
    public class EmailService : IEmailService
    {
        public void SendEmail()
        {
            using (var client = new SmtpClient("smtp.live.com"))
            {
                client.Port = 587;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential("info@taskever.com", "???");
                client.EnableSsl = true;
                client.Send("info@taskever.com", "hi_kalkan@yahoo.com", "Size task açýldý!", "Heyooo!");
            }
        }
    }
}