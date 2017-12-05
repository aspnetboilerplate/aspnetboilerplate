### Introduction

"***Cross-Site Request Forgery** (CSRF) is a type of attack that occurs
when a malicious web site, email, blog, instant message, or program
causes a user’s web browser to perform an unwanted action on a trusted
site for which the user is currently authenticated*"
([OWASP](https://www.owasp.org/index.php/Cross-Site_Request_Forgery_(CSRF)_Prevention_Cheat_Sheet)).

It's also briefly described
[here](http://www.asp.net/web-api/overview/security/preventing-cross-site-request-forgery-csrf-attacks)
to explain how to implement it in ASP.NET Web API.

ABP framework **simplifies** and **automates** CSRF protection as much
as possible. [Startup templates](/Templates) comes with
**pre-configured** and working out of the box. In this document, we will
explain how it's integrated to ASP.NET platforms and how it works.

#### Http Verbs

It's not needed to protect our actions for **GET**, **HEAD**,
**OPTIONS** and **TRACE** HTTP verbs since they should be side effect
free (don't change database) normally. While ABP assumes that (and
implements Anti Forgery protection for only **POST**, **PUT, PATCH** and
**DELETE** verbs), you can change this behaviour using attrbiutes
defined in this document.

#### Non Browser Clients

CSRF is a type of attack that is a problem for browsers. Because a
browser sends all cookies (including auth cookies) in all requests,
including cross domain requests. But, it's not a problem for non browser
clients, like mobile applications. ABP framework can understand the
difference and automatically **skips anti forgery validation for non
browser clients**.

### ASP.NET MVC

#### Features

ASP.NET MVC has it's own built-in AntiForgery system as you probably
know. But it has a few weeknesses:

-   Requires to add **ValidateAntiForgeryToken** attribute to all
    actions need to be protected. We may **forget** to add it for all
    needed actions.
-   ValidateAntiForgeryToken attribute only checks
    **\_\_RequestVerificationToken** in the HTML **form fields**. This
    makes very hard or impossible to use it for **AJAX** requests,
    especially if you are sending "**application/json**" as
    content-type. In AJAX requests, it's common to set token in the
    **request header**.
-   It's **hard to access** to the verification token in **javascript**
    code (especially if you don't write your javascript in .cshtml
    files). We need to access it to use it in our **AJAX** requests.
-   Even we can access to the token in javascript, we should **manually
    add** it to the header for every request.

ABP does followings to overcome this difficulties:

-   No need to add **ValidateAntiForgeryToken** attribute for **POST**,
    **PUT, PATCH** and **DELETE** actions anymore, because they are
    **automatically protected** (by **AbpAntiForgeryMvcFilter**).
    Automatic protection will be enough for most cases. But you can
    disable it for an action or controller using
    **DisableAbpAntiForgeryTokenValidation** attribute and you can
    enable it for any action/controller using
    **ValidateAbpAntiForgeryToken** attribute.
-   **AbpAntiForgeryMvcFilter** also checks token in the **header** in
    addition to HTML **form field**. Thus, we can easily use anti
    forgery token protection for AJAX requests.
-   Provides **abp.security.antiForgery.getToken()** function to get the
    token in the javascript, even you will not need it much.
-   **Automatically** adds anti forgery token to the **header** for all
    AJAX requests.

Thus, it almost seamlessly works.

#### Integration

Startup templates already integrated to CSRF protection out of the box.
If you need to manually add it to your project (maybe you created your
project before we added it), follow this guide.

##### Layout View

We should add the following code in our **Layout** view:

    @{
        SetAntiForgeryCookie();
    }

Thus, all pages use this layout will include it. This method is defined
in base ABP view class. It creates and sets appropriate token cookies
and makes javascript side working. If you have more than one layout, add
this to all of them.

That's all we should do for ASP.NET MVC applications. All AJAX requests
will work automatically. But we should still use
**@Html.AntiForgeryToken()** HTML helper for our **HTML forms** which
are **not posted via AJAX** (But **no need** to
ValidateAbpAntiForgeryToken attribute for the corresponding action).

#### Configuration

XSRF protection is **enabled by default**. You can disable or configure
it in your [module](Module-System.md)'s PreInitialize method. Example:

    Configuration.Modules.AbpWeb().AntiForgery.IsEnabled = false;

You can also configure token and cookie names using
*Configuration.Modules.AbpWebCommon().AntiForgery* object.

### ASP.NET Web API

#### Features

ASP.NET Web API **does not** include an anti forgery mechanism. ASP.NET
Boilerplate provides infrastructure to add CSRF protection for ASP.NET
Web API Controllers and completely automates it.

#### Integration

##### With ASP.NET MVC Clients

If you are using Web API inside an MVC project, **no additional
configuration needed**. Even if you are self-hosting your Web API layer
in another process, no configuration needed as long as you are making
AJAX requests from a configured MVC application.

##### With Other Clients

If your clients are diffrent kind of applications (say, an independent
angularjs application which can not use SetAntiForgeryCookie() method as
described before), then you should provide a way of setting the anti
forgery token cookie. One possible way of doing that is to create an api
controller like that:

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

Then you can call this action from client to set the cookie.

### ASP.NET Core

#### Features

**ASP.NET Core** MVC has a better [Anti
Forgery](https://docs.asp.net/en/latest/security/anti-request-forgery.md)
mechanism compared to previous version (ASP.NET MVC 5.x):

-   It has **AutoValidateAntiforgeryTokenAttribute** class that
    automates anti forgery validation for all **POST**, **PUT, PATCH**
    and **DELETE** actions.
-   It has **ValidateAntiForgeryToken** and **IgnoreAntiforgeryToken**
    attributes to control token validation.
-   Automatically adds anti forgery security token to HTML forms if you
    don't explicitly disable it. So, no need to call
    @Html.AntiForgeryToken() in most cases.
-   It can read request token from HTTP **header** and the **form
    field**.

ABP adds the following features:

-   **Automatically** adds anti forgery token to the **header** for all
    AJAX requests.
-   Provides **abp.security.antiForgery.getToken()** function to get the
    token in the javascript, even you will not need it much.

#### Integration

Startup templates already integrated to CSRF protection out of the box.
If you need to manually add it to your project (maybe you created your
project before we added it), follow this guide.

##### Startup Class

First, we should add AutoValidateAntiforgeryTokenAttribute to global
filters while adding MVC in ConfigureServices of Startup class:

    services.AddMvc(options =>
    {
        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    });

Thus, all MVC actions (except GET, HEAD, OPTIONS and TRACE as declared
before) will be automatically validated for anti forgery token.

##### Layout View

We should add the following code in our **Layout** view:

    @using Abp.Web.Security.AntiForgery
    @inject IAbpAntiForgeryManager AbpAntiForgeryManager
    @{
        AbpAntiForgeryManager.SetCookie(Context);
    }

Thus, all pages use this layout will include it. It creates and sets
appropriate token cookies and makes javascript side working. If you have
more than one layout, add this to all of them.

That's all we should do for ASP.NET Core MVC applications. All AJAX
requests will work automatically. For non-ajax form submits, ASP.NET
Core automatically adds anti forgery token field if you use one of
asp-\* tags in your form. So, no need to use @Html.AntiForgeryToken()
normally.

### Client Libraries

Anti forgery token should be provided in the request header for all AJAX
requests, as we declared before. We will see how it's done here.

#### jQuery

abp.jquery.js defines an AJAX interceptor which adds the anti forgery
token to the request header for every request. It gets the token from
**abp.security.antiForgery.getToken()** javascript function.

#### Angular

Angular automatically adds the anti forgery token to all AJAX requests.
See *Cross Site Request Forgery (XSRF) Protection* section in Angularjs
[$http document](https://docs.angularjs.org/api/ng/service/$http). ABP
uses the same cookie and header names as default. So, Angular
integration works out of the box.

#### Other Libraries

If you are using any other library for AJAX requests, you have three
options:

##### Intercept XMLHttpRequest

Since all libraries use the javascript's native AJAX object,
XMLHttpRequest, you can define such a simple interceptor to add token to
the header:

    (function (send) {
        XMLHttpRequest.prototype.send = function (data) {
            this.setRequestHeader(abp.security.antiForgery.tokenHeaderName, abp.security.antiForgery.getToken());
            return send.call(this, data);
        };
    })(XMLHttpRequest.prototype.send);

##### Use Library Interceptor

A good library provide interception points (like jquery and angularjs).
So, follow your vendor's documentation to learn how to intercept
requests and manipulate headers.

##### Add the Header Manually

The last option, you can use abp.security.antiForgery.getToken() to get
the token and add to the request header manually for every request. But
you probably do not need this and solve the problem as descibed above.

### Internals

You may wonder how ABP handles it. Actually, we are using the same
mechanism described in the angularjs documentation mentioned before. ABP
stores the token into a cookie (as described above) and sets requests
headers using that cookie. It also well integrates to ASP.NET MVC, Web
API and Core frameworks for validating it.
