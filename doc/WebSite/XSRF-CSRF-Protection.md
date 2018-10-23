### Introduction

"***Cross-Site Request Forgery** (CSRF) is a type of attack that occurs
when a malicious web site, email, blog, instant message, or program
causes a user’s web browser to perform an unwanted action on a trusted
site for which the user is currently authenticated*"
([OWASP](https://www.owasp.org/index.php/Cross-Site_Request_Forgery_(CSRF)_Prevention_Cheat_Sheet)).

It's also briefly described
[here](http://www.asp.net/web-api/overview/security/preventing-cross-site-request-forgery-csrf-attacks)
where it explains how to implement it into ASP.NET Web API.

ABP framework **simplifies** and **automates** CSRF protection as much
as possible. The [startup templates](/Templates) come with this
**pre-configured** and it works out-of-the-box. In this document, we will
explain how it's integrated into the ASP.NET platforms and how it works.

#### Http Verbs

You don't normally need to protect the the **GET**, **HEAD**,
**OPTIONS** and **TRACE** action HTTP verbs since they normally are side-effect
free (they don't change the database). While ABP assumes this and
implements Anti Forgery protection for only the **POST**, **PUT, PATCH** and
**DELETE** verbs, you can change this behavior using the attributes
defined in this document.

#### Non-Browser Clients

CSRF is a type of attack that is a problem for browsers because a
browser sends all cookies (including auth cookies) in all requests,
including cross-domain requests. This is not a problem for non-browser
clients, like mobile applications. The ABP framework understands the
difference and automatically **skips anti-forgery validation for non-
browser clients**.

### ASP.NET MVC

#### Features

ASP.NET MVC has its own built-in AntiForgery system, but there are a few weaknesses:

-   It requires you to add the **ValidateAntiForgeryToken** attribute to all
    actions that need to be protected. You could potentially **forget** to add
    it for all the needed actions!
-   The ValidateAntiForgeryToken attribute only checks the
    **\_\_RequestVerificationToken** in the HTML **form fields**. This
    makes it very hard or impossible to use it for **AJAX** requests,
    especially if you are sending "**application/json**" as the
    content-type. In AJAX requests, it's common to set the token in the
    **request header**.
-   It's **hard to access** the verification token in **JavaScript**
    (especially if you don't write your own JavaScript in .cshtml
    files). We need to access it to use it in our **AJAX** requests.
-   Even if we can access to the token in JavaScript, we must **manually
    add** it to the header for every request.

ABP does following things to overcome these problems:

-   You do not need to add the **ValidateAntiForgeryToken** attribute for **POST**,
    **PUT, PATCH** and **DELETE** actions anymore, because they are
    **automatically protected** (using **AbpAntiForgeryMvcFilter**).
    Automatic protection will be enough for most cases. But you can
    disable it for an action or controller using the
    **DisableAbpAntiForgeryTokenValidation** attribute and you can
    enable it for any action/controller using the
    **ValidateAbpAntiForgeryToken** attribute.
-   In addition to the HTML **form field**, **AbpAntiForgeryMvcFilter** also checks the token in the **header**.
    This way, we can easily use anti-forgery token protections for AJAX requests.
-   ABP provides the **abp.security.antiForgery.getToken()** function to get the
    token in JavaScript, even if you don't need it often.
-   ABP **Automatically** adds an anti-forgery token to the **header** for all
    AJAX requests.

In this way, CSRF protection works almost seamlessly.

#### Integration

The startup templates already integrate the CSRF protections out-of-the-box.
If you need to manually add it to your project (maybe you have a legacy project), follow this guide.

##### Layout View

We need to add the following code in our **Layout** view:

    @{
        SetAntiForgeryCookie();
    }

All pages that use this layout will include it. This method is defined
in the base ABP view class. It creates and sets the appropriate token cookies
and makes JavaScript do the side-work. If you have more than one layout, add
this to all of them.

That's all we have to do for ASP.NET MVC applications. All AJAX requests
will be protected automatically, but we should still use
the **@Html.AntiForgeryToken()** HTML helper for our **HTML forms** which
are **not posted via AJAX**. There is **no need** to add the
ValidateAbpAntiForgeryToken attribute for the corresponding action.

#### Configuration

XSRF protection is **enabled by default**. You can disable or configure
it in your [module](Module-System.md)'s PreInitialize method. Example:

    Configuration.Modules.AbpWeb().AntiForgery.IsEnabled = false;

You can also configure token and cookie names using
*Configuration.Modules.AbpWebCommon().AntiForgery* object.

### ASP.NET Web API

#### Features

The ASP.NET Web API **does not** include an anti-forgery mechanism. However, ASP.NET
Boilerplate provides the infrastructure to add automated CSRF protection for ASP.NET
Web API Controllers.

#### Integration

##### With ASP.NET MVC Clients

If you are using the Web API inside an MVC project, **no additional
configuration is needed**. Even if you are self-hosting your Web API layer
in another process, no configuration is needed as long as you are making
AJAX requests from a configured MVC application.

##### With Other Clients

If your clients are different kinds of applications (say, an independent
Angular application which can not use the SetAntiForgeryCookie() method as
described above), then you should provide a way of setting the anti-
forgery token cookie. One possible way of doing this is to create an api
controller like the following:

    using System.Net.Http;
    using Abp.Web.Security.AntiForgery;
    using Abp.WebApi.Controllers;

    namespace AngularForgeryDemo.Controllers
    {
        public class AntiForgeryController : AbpApiController
        {
            private readonly IAbpAntiForgeryManager _antiForgeryManager;

            public AntiForgeryController(IAbpAntiForgeryManager antiForgeryManager)
            {
                _antiForgeryManager = antiForgeryManager;
            }

            public HttpResponseMessage GetTokenCookie()
            {
                var response = new HttpResponseMessage();

                _antiForgeryManager.SetCookie(response.Headers);

                return response;
            }
        }
    }

You can then call this action from the client to set the cookie.

### ASP.NET Core

#### Features

**ASP.NET Core** MVC has a better [Anti
Forgery](<https://docs.microsoft.com/en-us/aspnet/core/security/anti-request-forgery?view=aspnetcore-2.0>)
mechanism compared to previous versions (ASP.NET MVC 5.x):

-   It has the **AutoValidateAntiforgeryTokenAttribute** class that
    automates anti-forgery validation for all **POST**, **PUT, PATCH**
    and **DELETE** actions.
-   It has the **ValidateAntiForgeryToken** and **IgnoreAntiforgeryToken**
    attributes to control token validation.
-   It automatically adds an anti-forgery security token to HTML forms if you
    don't explicitly disable it. So there's no need to call
    @Html.AntiForgeryToken() in most cases.
-   It can read the request token from the HTTP **header** and the **form
    field**.

ABP adds the following features:

-   ABP **automatically** adds an anti-forgery token to the **header** for all
    AJAX requests.
-   It also provides an **abp.security.antiForgery.getToken()** function to get the
    token in the JavaScript, even you will not need it much.

#### Integration

The startup templates are already integrated to use CSRF protections out-of-the-box.
If you need to manually add it to your project (maybe you created your
project before we added it), follow this guide.

##### Startup Class

First, we must add the AutoValidateAntiforgeryTokenAttribute to the global
filters while adding MVC in the ConfigureServices method of the Startup class:

    services.AddMvc(options =>
    {
        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    });

This way, all MVC actions (except GET, HEAD, OPTIONS and TRACE as declared
before) will be automatically validated for an anti-forgery token.

##### Layout View

We must add the following code in our **Layout** view:

    @using Abp.Web.Security.AntiForgery
    @inject IAbpAntiForgeryManager AbpAntiForgeryManager
    @{
        AbpAntiForgeryManager.SetCookie(Context);
    }

All the pages that use this layout will include it. It creates and sets
the appropriate token cookies and makes JavaScript do all the work. If you have
more than one layout, add this to all of them.

That's all we must do for ASP.NET Core MVC applications. All AJAX
requests will work automatically. For non-ajax form submits, ASP.NET
Core automatically adds an anti-forgery token field if you use one of
asp-\* tags in your form. So there's normally no need to use @Html.AntiForgeryToken().

### Client Libraries

The anti-forgery token must be provided in the request header for all AJAX
requests, as we declared above. We will see how it's done here.

#### jQuery

The abp.jquery.js script defines an AJAX interceptor which adds the anti-forgery
token to the request header for every request. It gets the token from the
**abp.security.antiForgery.getToken()** JavaScript function.

#### AngularJS

AngularJS automatically adds the anti-forgery token to all AJAX requests.
See the *Cross Site Request Forgery (XSRF) Protection* section in the AngularJS
[$http document](https://docs.angularjs.org/api/ng/service/$http). ABP
uses the same cookie and header names by default. So, Angular
integration works out of the box.

#### Other Libraries

If you are using any other library for AJAX requests, you have three
options:

##### Intercept XMLHttpRequest

Since all libraries use JavaScript's native AJAX object,
XMLHttpRequest, you can define a simple interceptor to add the token to
the header:

    (function (send) {
        XMLHttpRequest.prototype.send = function (data) {
            this.setRequestHeader(abp.security.antiForgery.tokenHeaderName, abp.security.antiForgery.getToken());
            return send.call(this, data);
        };
    })(XMLHttpRequest.prototype.send);

##### Using the Library Interceptor

A good library provides interception points (like jQuery and AngularJS),
so follow your vendor's documentation to learn how to intercept
requests and manipulate headers.

##### Add the Header Manually

As a final option, you can use the abp.security.antiForgery.getToken() method to get
the token and add it to the request header manually for every request.
You probably do not need this and can solve this problem by using the methods described above.

### Internals

You may wonder "How does ABP handle this?". Actually, we use the same
mechanism described in the AngularJS documentation mentioned before. ABP
stores the token into a cookie (as described above) and sets the request
headers using that cookie. For validating it, it also integrates well into the
ASP.NET MVC, Web API and Core frameworks.
