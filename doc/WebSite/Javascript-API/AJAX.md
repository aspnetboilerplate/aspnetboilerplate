### Problems of AJAX Operations

Performing an AJAX call is frequently used by modern applcations.
Especially in SPAs (Single-Page Applications), it's almost the only way
of communicating with the server. An AJAX call consists of some
repeating steps:

In client side, basically, javascript code should supply a URL,
optionally a data and select a method (POST, GET...) to perfom an AJAX
call. It must wait and handle the return value. There may be an error
(network error generally) while performing call to the server. Or there
may be some error in server side and server may send an failed response
with an error message. Client code should handle these and optionally
notify user (may show an error dialog). If there is no error and server
sent a return data, client must also handle it. Also, generally you want
to block some (or whole) area of the screen and show a busy indicator
until AJAX operation complete.

Server code should get the request, perform some server-side code, catch
any exceptions and return a valid response to the client. In an error
situation, it may optionally send an error message to the client. If
it's a validation error, server may want to add validation problems. In
success case, it may send a return value to the client.

### ASP.NET Boilerplate Way

ASP.NET Boilerplate automates some of these steps by wrapping AJAX calls
with **abp.ajax** function. An example ajax call:

    var newPerson = {
        name: 'Dougles Adams',
        age: 42
    };

    abp.ajax({
        url: '/People/SavePerson',
        data: JSON.stringify(newPerson)
    }).done(function(data) {
        abp.notify.success('created new person with id = ' + data.personId);
    });

abp.ajax gets **options** as an object. You can pass any parameter that
is valid in jQuery's [$.ajax](http://api.jquery.com/jQuery.ajax/)
method. There are some **defaults** here: dataType is '**json**', type
is '**POST**' and contentType is '**application/json**' (So, we're
calling JSON.stringify to convert javascript object to JSON string
before sending to the server). You can override defaults by passing
options to abp.ajax.

abp.ajax returns **[promise](http://api.jquery.com/deferred.promise/)**.
So, you can write done, fail, then... handlers. In this example, we made
a simple AJAX request to **PeopleController**'s **SavePerson** action.
In the **done** handler, we got the database **id** for the new created
person and showing a success notification (See [notification
API](/Pages/Documents/Javascript-API/Notification)). Let's see the **MVC
controller** for this AJAX call:

    public class PeopleController : AbpController
    {
        [HttpPost]
        public JsonResult SavePerson(SavePersonModel person)
        {
            //TODO: save new person to database and return new person's id
            return Json(new {PersonId = 42});
        }
    }

**SavePersonModel** contains Name and Age properties as you can guess.
SavePerson is marked with **HttpPost** since abp.ajax's default method
is POST. I simplified method implemtation by returning an anonymous
object.

This seams straightforward, but there are some important things behind
the scenes that is handled by ASP.NET Boilerplate. Let's dive into
details...

#### AJAX Return Messages

Even we directly return an object with PersonId = 2, ASP.NET Boilerplate
wraps it by an **MvcAjaxResponse** object. Actual AJAX response is
something like that:

    {
      "success": true,
      "result": {
        "personId": 42
      },
      "error": null,
      "targetUrl": null,
      "unAuthorizedRequest": false,
      "__abp": true
    }

Here, all properties are camelCase (since it's conventional in
javascript world) even they are PascalCase in the server code. Let's
explain all fields:

-   **success**: A boolean value (true or false) that indicates success
    status of the operation. If this is true, abp.ajax resolves the
    promise and calls the **done** handler. If it's false (if there is
    an exception thrown in method call), it calls **fail** handler and
    shows the **error** message using
    [abp.message.error](/Pages/Documents/Javascript-API/Message)
    function.
-   **result**: Actual return value of the controller action. It's valid
    if success is true and server sent a return value.
-   **error**: If success is false, this field is an object that
    contains **message** and **details** fields.
-   **targetUrl**: This provides a possibility to the server to
    **redirect** client to another url if needed.
-   **unAuthorizedRequest**: This provides a possibility to the server
    to inform client that this operation is not authorized or user is
    not authenticated. abp.ajax **reloads** current page if this is
    true.
-   **\_\_abp**: A special signature that is returned by ABP wrapped
    responses. You don't use this yourself, but abp.ajax handles it.

This return format is recognized and handled by **abp.ajax** function.
Your done handler in abp.ajax gets the actual return value of the
controller (An object with personId property) if there is no error.

#### Handling Errors

As described above, ASP.NET Boilerplate handles exceptions in server and
returns an object with an error message like that:

    {
      "targetUrl": null,
      "result": null,
      "success": false,
      "error": {
        "message": "An internal error occured during your request!",
        "details": "..."
      },
      "unAuthorizedRequest": false,
      "__abp": true
    }

As you see, **success is false** and **result is null**. abp.ajax
handles this object and shows an error message to the user using
[abp.message.error](/Pages/Documents/Javascript-API/Message) function.
If your server side code throws an exception type of
**UserFriendlyException**, it directly shows exception's message to the
user. Otherwise, it hides the actual error (writes error to logs) and
shows a standard ''An internal error occured..." message to the user.
All these are automatically done by ASP.NET Boilerplate.

You may want to disable displaying message for particular AJAX call.
Then add **abpHandleError: false** into abp.ajax options.

##### HTTP Status Codes

ABP returns given HTTP status codes on exceptions:

-   **401** for unauthenticated requests (Used has not logged in but
    server action needs authentication).
-   **403** for unauthorized requests.
-   **500** for all other exception types.

#### WrapResult and DontWrapResult Attributes

You can control wrapping using **WrapResult** and **DontWrapResult**
attributes for an action or all actions of a controller.

##### ASP.NET MVC Controllers

ASP.NET Boilerplate **wraps** (as described above) ASP.NET **MVC**
action results **by default** if return type is **JsonResult** (or
Task&lt;JsonResult&gt; for async actions). You can change this by using
**WrapResult** attribute as shown below:

    public class PeopleController : AbpController
    {
        [HttpPost]
        [WrapResult(WrapOnSuccess = false, WrapOnError = false)]
        public JsonResult SavePerson(SavePersonModel person)
        {
            //TODO: save new person to database and return new person's id
            return Json(new {PersonId = 42});
        }
    }

As a shortcut, we can only use **\[DontWrapResult\]** which is identical
for this example.

You can change this default behaviour from [startup
configuration](../Startup-Configuration.md) (using
Configuration.Modules.AbpMvc()...).

##### ASP.NET Web API Controllers

ASP.NET Boilerplate **does not wrap** Web API actions **by default** if
action has successfully executed. You can add WrapResult to actions or
controllers if you need. But it **wraps exceptions**.

You can change this default behaviour from [startup
configuration](../Startup-Configuration.md) (using
Configuration.Modules.AbpWebApi()...).

##### Dynamic Web API Layer

ASP.NET Boilerplate **wraps** dynamic web api layer methods **by
default**. You can change this behaviour using **WrapResult** and
**DontWrapResult** attributes in the **interface** of your application
service.

You can change this default behaviour from [startup
configuration](../Startup-Configuration.md) (using
Configuration.Modules.AbpWebApi()...).

##### ASP.NET Core Controllers

ABP automatically wraps results for JsonResult, ObjectResult and any
object which does not implement IActionResult.  See [ASP.NET Core
documentation](../AspNet-Core.md) for more.

You can change this default behaviour from [startup
configuration](../Startup-Configuration.md) (using
Configuration.Modules.AbpAspNetCore()...).

#### Dynamic Web API Layer

While ASP.NET Boilerplate provides a mechanism to make easy AJAX calls,
in a real world application it's typical to write a javascript function
for every AJAX call. For example:

    //Create a function to abstract AJAX call
    var savePerson = function(person) {
        return abp.ajax({
            url: '/People/SavePerson',
            data: JSON.stringify(person)
        });
    };

    //Create a new person
    var newPerson = {
        name: 'Dougles Adams',
        age: 42
    };

    //Save the person
    savePerson(newPerson).done(function(data) {
        abp.notify.success('created new person with id = ' + data.personId);
    });

This is a good practice but time-consuming and tedious to write a
function for every ajax call. ASP.NET can automatically generate these
type of functions for [application
service](/Pages/Documents/Application-Services) and controllers.

Read the [dynamic web api](/Pages/Documents/Dynamic-Web-API) layer
documentation for Web API and ASP.NET Core documentation for [ASP.NET
Core](../AspNet-Core.md) integration.
