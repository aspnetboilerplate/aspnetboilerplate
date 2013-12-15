using System.Net.Mail;
using System.Text;
using Taskever.Domain.Entities;

namespace Taskever.Application.Services
{
    public class CompletedTaskNotification : INotification
    {
        public Task Task { get; private set; }

        public CompletedTaskNotification(Task task)
        {
            Task = task;
        }

        public MailMessage CreateMailMessage()
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(Task.CreatorUser.EmailAddress);
            mail.IsBodyHtml = true;
            mail.Subject = "Completed a task: " + Task.Title;
            mail.SubjectEncoding = Encoding.UTF8;

            var mailBuilder = new StringBuilder();
            mailBuilder.Append(
                @"<!DOCTYPE html>
<html lang=""en"" xmlns=""http://www.w3.org/1999/xhtml"">
<head>
    <meta charset=""utf-8"" />
    <title>{TEXT_HTML_TITLE}</title>
    <style>
        body {
            font-family: Verdana, Geneva, 'DejaVu Sans', sans-serif;
            font-size: 12px;
        }
    </style>
</head>
<body>
    <h3>{TEXT_ASSIGNED_TASK_HEADER}</h3>
    <h4><b>{TEXT_TITLE}</b>: <a href=""http://www.taskever.com/#task/{TASK_ID}"">{TASK_TITLE}</a></h4>
    <p>
        <b>{TEXT_DETAILS}</b>: {TASK_DETAILS}<br />
        <b>{TEXT_PRIORITY}</b>: {TASK_PRIORITY}<br />
        <b>{TEXT_COMPLETED_BY}</b>: {TASK_COMPLETED_BY}<br />
    </p>
    <p>&nbsp;</p>
    <p><a href=""http://www.taskever.com"">http://www.taskever.com</a></p>
    <p><small>{TEXT_NOTE_DISABLE_EMAILS}</small></p>
</body>
</html>");

            mailBuilder.Replace("{TEXT_HTML_TITLE}", "Completed a task: " + Task.Title);
            mailBuilder.Replace("{TEXT_ASSIGNED_TASK_HEADER}", "A task (that is created by you) in Taskever is COMPLETED");
            mailBuilder.Replace("{TEXT_TITLE}", "Title");
            mailBuilder.Replace("{TASK_TITLE}", Task.Title);
            mailBuilder.Replace("{TASK_ID}", Task.Id.ToString());
            mailBuilder.Replace("{TEXT_DETAILS}", "Details");
            mailBuilder.Replace("{TASK_DETAILS}", Task.Description);
            mailBuilder.Replace("{TEXT_PRIORITY}", "Priority");
            mailBuilder.Replace("{TASK_PRIORITY}", Task.Priority.ToString());
            mailBuilder.Replace("{TEXT_COMPLETED_BY}", "Completed by");
            mailBuilder.Replace("{TASK_COMPLETED_BY}", Task.AssignedUser.NameAndSurname);
            mailBuilder.Replace("{TEXT_NOTE_DISABLE_EMAILS}", "NOTE: If you don't want to receive emails from Taskever, you can disable emails in settings from taskever.com");

            mail.Body = mailBuilder.ToString();
            mail.BodyEncoding = Encoding.UTF8;

            return mail;
        }
    }
}