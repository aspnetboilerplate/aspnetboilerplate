### Introduction

Sending emails is a very common task for most applications.
ASP.NET Boilerplate provides the basic infrastructure to send
emails in a simple way. It also separates the email server configuration from the sending 
of emails.

### IEmailSender

**IEmailSender** is a service to send emails without knowing the details. Example usage:

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

We simply [injected](Dependency-Injection.md) **IEmailSender** and
used the **Send** method. The Send method has additional overloads. For example, it
can also get a MailMessage object (not available for .NET Core since .NET Core
does not include SmtpClient and MailMessage).

#### ISmtpEmailSender

There is also an **ISmtpEmailSender** which extends IEmailSender and adds a
**BuildClient** method to create an **SmtpClient** and then directly uses it
(not available for .NET Core since .NET Core does not include SmtpClient
and MailMessage). Using IEmailSender will be enough for most cases.

#### NullEmailSender

There is also a [null object
pattern](https://en.wikipedia.org/wiki/Null_Object_pattern)
implementation of IEmailSender, aptly named **NullEmailSender**. You can use it in
unit tests or inject IEmalSender with the [property
injection](Dependency-Injection.md) pattern.

### Configuration

Email Sender uses a [settings management](Setting-Management.md) system
to read email-sending configurations. All the setting names are defined in the
Abp.Net.Mail.EmailSettingNames class as constant strings. 

Their values and descriptions:

-   Abp.Net.Mail.**DefaultFromAddress**: Used as the sender's email address
    when you don't specify a sender when sending emails (just like in the
    example above).
-   Abp.Net.Mail.**DefaultFromDisplayName**: Used as the sender's display name
    when you don't specify a sender when sending emails (just like in the
    example above).
-   Abp.Net.Mail.**Smtp.Host**: The IP/Domain of the SMTP server (default:
    127.0.0.1).
-   Abp.Net.Mail.**Smtp.Port**: The Port of the SMTP server (default: 25).
-   Abp.Net.Mail.**Smtp.UserName**: Username, if the SMTP server requires
    authentication.
-   Abp.Net.Mail.**Smtp.Password**: Password, if the SMTP server requires
    authentication.
-   Abp.Net.Mail.**Smtp.Domain**: Domain for the username, if the SMTP
    server requires authentication.
-   Abp.Net.Mail.**Smtp.EnableSsl**: A value that indicates if the SMTP server
    uses SSL or not ("true" or "false". Default: "false").
-   Abp.Net.Mail.**Smtp.UseDefaultCredentials**: If true, uses default
    credentials instead of the provided username and password ("true" or
    "false". Default: "true").

### MailKit Integration

Since .NET Core does not support the standard System.Net.Mail.SmtpClient, 
we need a 3rd-party vendor to send emails. Fortunately,
[MailKit](https://github.com/jstedfast/MailKit) provides a good
replacement for the default SmtpClient. It's also
[suggested](https://www.infoq.com/news/2017/04/MailKit-MimeKit-Official)
by Microsoft.

The Abp.MailKit package gracefully integrates in to ABP's email sending system, so you 
can still use IEmailSender as described above to send emails via MailKit.

#### Installation

First, install the [Abp.MailKit](https://www.nuget.org/packages/Abp.MailKit)
NuGet package to your project:

    Install-Package Abp.MailKit

#### Integration

Add the AbpMailKitModule to the dependencies of your
[module](Module-System.md):

    [DependsOn(typeof(AbpMailKitModule))]
    public class MyProjectModule : AbpModule
    {
        //...
    }

#### Usage

You can use **IEmailSender** as described above since the Abp.MailKit
package [registers](Dependency-Injection.md) the MailKit implementation
for it. It also uses the same configuration.

#### Customization

You may need to make additional configuration or customizations while
creating MailKit's SmtpClient. In that case, you can
[replace](Startup-Configuration.md) the IMailKitSmtpBuilder interface with
your own implementation. You can derive from the DefaultMailKitSmtpBuilder
to make it easier. For instance, you may want to accept all SSL
certificates. In that case, you can override the ConfigureClient method as
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

You can then replace the IMailKitSmtpBuilder interface with your
implementation in the [PreInitialize](Module-System.md) method of your
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

(Don't forget to add the "using Abp.Configuration.Startup;" statement since the
ReplaceService extension method is defined in that namespace)
