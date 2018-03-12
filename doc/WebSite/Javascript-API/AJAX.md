### Problems of AJAX Operations

Performing AJAX calls is frequently used by modern applcations.
Especially in SPAs (Single Page Applications), it's almost the only way
of communicating with the server. An AJAX call consists of some
repeating steps:

On the client-side, the JavaScript code should supply an URL,
optionally some data, and it should select a method (POST, GET...) to perfom an AJAX
call. It must wait and handle the return value. There may be an error
(network error generally) while performing a call to the server. Or there
may be some error on the server-side and server may send a failed response
with an error message. The client-side code must handle these and optionally
notify a user (like show an error dialog). If there is no error and the server
returns data, the client must also handle it. In addition to this, you generally want
to block some part or a whole area of the screen and show a busy indicator
until an AJAX operation completes.

Server-code should get the request, perform some server-side code, catch
exceptions and return a valid response to the client. If an error
occurs, it may optionally send an error message to the client. If
it's a validation error, the server may want to the add descriptions to validation problems. 
In the case of a successful request, it may send return values to the client.

### The ASP.NET Boilerplate Way

ASP.NET Boilerplate automates some of these steps by wrapping AJAX calls
with the **abp.ajax** function. An example ajax call:

    var newPerson = {
        name: 'Dougles Adams',
        age: 42
    };

    abp.ajax({
        url: '/People/SavePerson',
        data: JSON.stringify(newPerson)
    }).done(function(data) {
        abp.notify.success('Created new person with id = ' + data.personId);
    });

abp.ajax gets **options** as an object. You can pass any valid parameter 
into jQuery's [$.ajax](http://api.jquery.com/jQuery.ajax/) method. 
There are some **defaults** here: the dataType is '**json**', the type
is '**POST**', and the contentType is '**application/json**' (We're
calling JSON.stringify to convert a JavaScript object into a JSON string
before sending it to the server). You can override these defaults by passing
options to abp.ajax.

abp.ajax returns a **[promise](http://api.jquery.com/deferred.promise/)**,
so you can write done, fail, then (etc) handlers. In this example, we made
a simple AJAX request to the **PeopleController**'s **SavePerson** action.
In the **done** handler, we fetched the database **id** for the newly created
person and showed a success notification (See [notification
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

The **SavePersonModel** contains the Name and Age properties.
The SavePerson action is marked with **HttpPost**, since abp.ajax's default method
is POST. We simplified the method implementation by returning an anonymous
object.

This seams pretty straightforward, but there are some important things behind
the scenes that are handled by ASP.NET Boilerplate. Let's dive into those
details...

#### AJAX Return Messages

When we directly return an object with PersonId = 2, ASP.NET Boilerplate
wraps it with an **MvcAjaxResponse** object. The actual AJAX response is
something like this:

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

Here all the properties are camelCase (since it's conventional in
JavaScript) even if they are PascalCase on the server-side's code. Here's
an explanation of all the fields:

-   **success**: A boolean value (true or false) that indicates the success
    status of the operation. If this is true, abp.ajax resolves the
    promise and calls the **done** handler. If it's false (if there is
    an exception thrown in the method call), it calls the **fail** handler and
    shows the **error** message using the
    [abp.message.error](/Pages/Documents/Javascript-API/Message)
    function.
-   **result**: The actual return value of the controller action. It's valid
    if the request was a success and if the server sent a return value.
-   **error**: If success is false, this field is an object that
    contains the **message** and **details** fields.
-   **targetUrl**: This provides a way for the server to
    **redirect** the client to another url if needed.
-   **unAuthorizedRequest**: This provides a method for the server
    to inform the client that this operation is not authorized or the user is
    not authenticated. abp.ajax **reloads** the current page if this is
    true.
-   **\_\_abp**: A special signature that is returned by an ABP wrapped
    responses. You don't use this yourself, but abp.ajax handles it.

This return format is recognized and handled by the **abp.ajax** function.
The done handler in abp.ajax gets the actual return value of the
controller (An object with a personId property) if there is no error.

#### Handling Errors

As described above, ASP.NET Boilerplate handles exceptions on the server and
returns an object with an error message like this:

    {
      "targetUrl": null,
      "result": null,
      "success": false,
      "error": {
        "message": "An internal error occurred during your request!",
        "details": "..."
      },
      "unAuthorizedRequest": false,
      "__abp": true
    }

As you can see, **success is false** and **result is null**. abp.ajax
handles this object and shows an error message to the user using the
[abp.message.error](/Pages/Documents/Javascript-API/Message) function.
If your server-side code throws an exception type of
**UserFriendlyException**, it directly shows the exception's message to the
user. Otherwise, it hides the actual error (writes error to logs) and
shows a standard ''An internal error occurred..." message to the user.
All these are automatically done by ASP.NET Boilerplate.

You may want to disable displaying the message for a particular AJAX call.
If so, add **abpHandleError: false** into the abp.ajax options.

##### HTTP Status Codes

ABP returns the following HTTP status codes when exceptions occur:

-   **401** for unauthenticated requests (Used has not logged in and the
    server action needs authentication).
-   **403** for unauthorized requests.
-   **500** for all other exception types.

#### WrapResult and DontWrapResult Attributes

You can control the wrapping using **WrapResult** and the **DontWrapResult**
attributes for an action or all actions in a controller.

##### ASP.NET MVC Controllers

ASP.NET Boilerplate **wraps** (as described above) ASP.NET **MVC**
action results **by default** if the return type is a **JsonResult** (or
Task&lt;JsonResult&gt; for async actions). You can change this by using the
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

As a shortcut, we can simply use the **\[DontWrapResult\]** attribute which is identical
for this example.

You can change this default behaviour from the [startup
configuration](../Startup-Configuration.md) (using
Configuration.Modules.AbpMvc()...).

##### ASP.NET Web API Controllers

ASP.NET Boilerplate **does not wrap** Web API actions **by default** if
an action has successfully executed. You can add WrapResult to actions or
controllers if you need to, but by default it **wraps exceptions**.

You can change this default behavior from the [startup
configuration](../Startup-Configuration.md) (using
Configuration.Modules.AbpWebApi()...).

##### Dynamic Web API Layer

ASP.NET Boilerplate **wraps** dynamic web api layer methods **by
default**. You can change this behavior using the **WrapResult** and
**DontWrapResult** attributes in the **interface** of your application
service.

You can change this default behaviour from the [startup
configuration](../Startup-Configuration.md) (using
Configuration.Modules.AbpWebApi()...).

##### ASP.NET Core Controllers

ABP automatically wraps results for a JsonResult, ObjectResult and any
object which does not implement IActionResult.  See the [ASP.NET Core
documentation](../AspNet-Core.md) for more info.

You can change this default behavior from the [startup
configuration](../Startup-Configuration.md) (using
Configuration.Modules.AbpAspNetCore()...).

#### Dynamic Web API Layer

While ASP.NET Boilerplate provides a mechanism to make AJAX calls easy,
in a real-world application it's typical to write a JavaScript function
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

This is good practice, but time-consuming and tedious, because you have to write a
function for every ajax call. ASP.NET can automatically generate these
type of functions for [application
services](/Pages/Documents/Application-Services) and controllers.

Read the [dynamic web api](/Pages/Documents/Dynamic-Web-API) layer
documentation for the Web API and ASP.NET Core documentation for the [ASP.NET
Core](../AspNet-Core.md) integration.
