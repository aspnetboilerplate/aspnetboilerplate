### Introduction

Email sending is a pretty common task for almost every application.
ASP.NET Boilerplate provides a basic infrastructure to simply send
emails and seperate email server configuration from sending emails.

### IEmailSender

**IEmailSender** is a service to simply send emails without knowing
details. Example usage:

    public class TaskManager : IDomainService
    {
        private readonly IEmailSender _emailSender;

        public TaskManager(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public void Assign(Task task, Person person)
        {
            //Assign task to the person
            task.AssignedTo = person;

            //Send a notification email
            _emailSender.Send(
                to: person.EmailAddress,
                subject: "You have a new task!",
                body: $"A new task is assigned for you: <b>{task.Title}</b>",
                isBodyHtml: true
            );
        }
    }

We simply [injected](Dependency-Injection.html) **IEmailSender** and
used **Send** method. Send method has a few more overloads. It can also
get a MailMessage object (not available for .net core since .net core
does not include SmtpClient and MailMessage).

#### ISmtpEmailSender

There is also **ISmtpEmailSender** which extends IEmailSender and adds
**BuildClient** method to create an **SmtpClient** to directly use it
(not available for .net core since .net core does not include SmtpClient
and MailMessage). Using IEmailSender will be enough for most cases.

#### NullEmailSender

There is also a [null object
pattern](https://en.wikipedia.org/wiki/Null_Object_pattern)
implementation of IEmailSender as **NullEmailSender**. You can use it in
unit tests or injecting IEmalSender with [property
injection](Dependency-Injection.html) pattern.

### Configuration

Email Sender uses [setting management](Setting-Management.html) system
to read emal sending configuration. All setting names are defined in
Abp.Net.Mail.EmailSettingNames class as constant strings. Their values
and descriptions:

-   Abp.Net.Mail.**DefaultFromAddress**: Used as sender email address
    when you don't specify a sender while sending emails (as like in the
    sample above).
-   Abp.Net.Mail.**DefaultFromDisplayName**: Used as sender display name
    when you don't specify a sender while sending emails (as like in the
    sample above).
-   Abp.Net.Mail.**Smtp.Host**: IP/Domain of the SMTP server (default:
    127.0.0.1).
-   Abp.Net.Mail.**Smtp.Port**: Port of the SMTP server (default: 25).
-   Abp.Net.Mail.**Smtp.UserName**: Username, if SMTP server requires
    authentication.
-   Abp.Net.Mail.**Smtp.Password**: Password, if SMTP server requires
    authentication.
-   Abp.Net.Mail.**Smtp.Domain**: Domain for the username, if SMTP
    server requires authentication.
-   Abp.Net.Mail.**Smtp.EnableSsl**: A value indicates that SMTP server
    uses SSL or not ("true" or "false". Default: "false").
-   Abp.Net.Mail.**Smtp.UseDefaultCredentials**: True, to use default
    credentials instead of provided username and password ("true" or
    "false". Default: "true").

### MailKit Integration

Since .net core does not support standard System.Net.Mail.SmtpClient, so
we need a 3rd-party vendor to send emails. Fortunately,
[MailKit](https://github.com/jstedfast/MailKit) provides a good
replacement for default SmtpClient. It's also
[suggested](https://www.infoq.com/news/2017/04/MailKit-MimeKit-Official)
by Microsoft.

Abp.MailKit package gracefully integrates to ABP's email sending system.
So, you can still use IEmailSender as described above to send emails via
MailKit.

#### Installation

First, install [Abp.MailKit](https://www.nuget.org/packages/Abp.MailKit)
nuget package to your project:

    Install-Package Abp.MailKit

#### Integration

Add AbpMailKitModule to dependencies of your
[module](Module-System.html):

    [DependsOn(typeof(AbpMailKitModule))]
    public class MyProjectModule : AbpModule
    {
        //...
    }

#### Usage

You can use **IEmailSender** as described before since Abp.MailKit
package [registers](Dependency-Injection.html) MailKit implementation
for it. It also uses the same configuration defined above.

#### Customization

You may need to make additional configuration or customization while
creating MailKit's SmtpClient. In that case, you can
[replace](Startup-Configuration.html) IMailKitSmtpBuilder interface with
your own implementation. You can derive from DefaultMailKitSmtpBuilder
to make it easier. For instance, you may want to accept all SSL
certificates. In that case, you can override ConfigureClient method as
shown below:

    public class MyMailKitSmtpBuilder : DefaultMailKitSmtpBuilder
    {
        public MyMailKitSmtpBuilder(ISmtpEmailSenderConfiguration smtpEmailSenderConfiguration) 
            : base(smtpEmailSenderConfiguration)
        {
        }

        protected override void ConfigureClient(SmtpClient client)
        {
            client.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;

            base.ConfigureClient(client);
        }
    }

Then you can replace IMailKitSmtpBuilder interface with your
implementation in [PreInitialize](Module-System.html) method of your
module:

    [DependsOn(typeof(AbpMailKitModule))]
    public class MyProjectModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.ReplaceService<IMailKitSmtpBuilder, MyMailKitSmtpBuilder>();
        }

        //...
    }

(remember to add "using Abp.Configuration.Startup;" statement since
ReplaceService extension method is defined in that namespace)
