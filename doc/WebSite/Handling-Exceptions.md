### Introduction

This document is for ASP.NET MVC and Web API. If you're interested in
ASP.NET Core, see [ASP.NET Core](AspNet-Core.md) documentation.

In a web application, exceptions are usually handled in MVC Controller
actions and Web API Controller actions. When an exception occurs,
application user somehow informed about the error and optionally reason
of the error.

If an error occured in a regular HTTP request, an error page is shown.
If an error occured in an AJAX request, server sends error information
to the client and then client handles and shows the error to the user.

Handling exceptions in all web request is a tedious and repeating work.
ASP.NET Boilerplate **automates** this. You almost never need to
explicitly handle any exception. ASP.NET Boilerplate handles all
exceptions, logs them and returns appropriate and formatted response to
the client. Also, handles these response in the client and show error
messages to the user.

### Enabling Error Handling

To enable error handling for ASP.NET MVC Controllers, **customErrors**
mode must be enabled for ASP.NET MVC applications.

    <customErrors mode="On" />

It also can be '**RemoteOnly**' if you do not want to handler errors in
local computer. Note that this is only required for ASP.NET MVC
Controllers, not needed for Web API Controllers.

If you have already handling exceptions in a global filter then it may
hide exceptions and ABP's exception handling may not work as you
expected. So, if you do that, do it carefully.

### Non-Ajax Requests

If request is not AJAX, an error page is shown.

#### Showing Exceptions

Here, there is an MVC controller action which throws an arbitrary
exception:

    public ActionResult Index()
    {
        throw new Exception("A sample exception message...");
    }

Surely, this exception could be thrown by another method that is called
from this action. ASP.NET Boilerplate handles this exception, logs it
and shows '**Error.cshtml**' view. You can **customize** this view to
show the error. An **example** Error view (default Error view in ASP.NET
Boilerplate templates):

<img src="images/error-page-default.png" alt="Default Error view" class="img-thumbnail" />

ASP.NET Boilerplate hides details of the exception from users and shows
a standard (and localizable) error message, unless you explicitly throw
a **UserFriendlyException**.

#### UserFriendlyException

UserFriendlyException is a special type of exception that is directly
shown to the user. See the sample below:

    public ActionResult Index()
    {
        throw new UserFriendlyException("Ooppps! There is a problem!", "You are trying to see a product that is deleted...");
    }

ASP.NET Boilerplate logs it but does not hide exception in this time:

<img src="images/error-page-user-friendly.png" alt="User friendly exception" class="img-thumbnail" />

So, if you want to show a special error message to users, just throw a
UserFriendlyException (or an exception derived from it).

#### Error Model

ASP.NET Boilerplate passes an **ErrorViewModel** object as model to the
Error view:

    public class ErrorViewModel
    {
        public AbpErrorInfo ErrorInfo { get; set; }

        public Exception Exception { get; set; }
    }

**ErrorInfo** contains detailed informations about the error that can be
shown to the user. **Exception** object is the thrown exception. You can
check it and show additional informations if you want. For example, we
can show validation errors if it's an **AbpValidationException**:

<img src="images/error-page-validation.png" alt="Validation errors" class="img-thumbnail" />

### AJAX Requests

If return type of MVC action is JsonResult (or Task&lt;JsonResult for
async actions), ASP.NET Boilerplate returns a JSON object to the client
on exceptions. Sample return object for an error:

    {
      "targetUrl": null,
      "result": null,
      "success": false,
      "error": {
        "message": "An internal error occured during your request!",
        "details": "..."
      },
      "unAuthorizedRequest": false
    }

**success: false** indicates that there is an error. **error** object
provides error **message** and **details**.

When you use ASP.NET Boilerplate's infrastructure to make AJAX request
in client side, it automatically handles this JSON object and shows
error message to the user using [message A<span
class="text-primary">PI</span>](/Pages/Documents/Javascript-API/Message).
See [AJAX API](/Pages/Documents/Javascript-API/AJAX) for more
information.

### Exception Event

When ASP.NET Boilerplare handles any exception, it triggers
**AbpHandledExceptionData** event that can be registered to be informed
(See [eventbus documentation](/Pages/Documents/EventBus-Domain-Events)
for more information about Event Bus). Example:

    public class MyExceptionHandler : IEventHandler<AbpHandledExceptionData>, ITransientDependency
    {
        public void HandleEvent(AbpHandledExceptionData eventData)
        {
            //TODO: Check eventData.Exception!
        }
    }

If you put this example class into your application (generally into your
Web project), **HandleEvent** method will be called for all exceptions
handled by ASP.NET Boilerplate. So, you can investigate the Exception
object in detail.
