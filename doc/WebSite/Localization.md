### Introduction

Developing a world-ready application, including an application that can be localized into one or more languages, requires localization features.
ASP.NET Boilerplate provides extensive support for the development of world-ready and localized applications.

### Application Languages

The first thing to do is to declare which languages are supported. This is
done in the **PreInitialize** method of your
[module](/Pages/Documents/Module-System) as shown below:

    Configuration.Localization.Languages.Add(new LanguageInfo("en", "English", "famfamfam-flags gb", true));
    Configuration.Localization.Languages.Add(new LanguageInfo("tr", "Türkçe", "famfamfam-flags tr"));

On the server side, you can [inject](/Pages/Documents/Dependency-Injection)
and use the **ILocalizationManager**. On the client side, you can use the
**abp.localization** JavaScript API to get a list of all available
languages, as well as the current language. "famfamfam-flags gb" (and "famfamfam-flags tr") is
just a CSS class, which you can change based on your needs. You can then use it
in the UI to show the related flag.

The ASP.NET Boilerplate [templates](/Templates) use this system to show a
**language-switch** combobox to the user. Create a template and
see the source code for more info.

### Localization Sources

Localization texts can be stored in different sources. You can even use
more than one source in the same application (If you have more than one
[module](/Pages/Documents/Module-System), each module can define a
separated localization source, or one module can define multiple
sources). The **ILocalizationSource** interface should be implemented by a
localization source. It is then **register**ed to ASP.NET Boilerplate's
localization configuration.

Each localization source must have a **unique source name**. There are
pre-defined localization source types, as defined below.

#### XML Files

Localization texts can be stored in XML files. The content of an XML file is
something like this:

    <?xml version="1.0" encoding="utf-8" ?>
    <localizationDictionary culture="en">
      <texts>
        <text name="TaskSystem" value="Task System" />
        <text name="TaskList" value="Task List" />
        <text name="NewTask" value="New Task" />
        <text name="Xtasks" value="{0} tasks" />
        <text name="CompletedTasks" value="Completed tasks" />
        <text name="EmailWelcomeMessage">Hi,
    Welcome to Simple Task System! This is a sample
    email content.</text>
      </texts>
    </localizationDictionary>

XML files must be unicode (**utf-8**). **culture="en"** declares that
this XML file contains English texts. For text nodes; the **name** attribute
is used to identify a text. You can use the **value** attribute or **inner
text** (like the last one) to set the value of the localization text. We
create a separated XML file for **each language** as shown below:

<img src="images/localization_files2.png" alt="Localization files" class="img-thumbnail" />

**SimpleTaskSystem** is the **source name** here and
SimpleTaskSystem.xml defines the **default language**. When a text is
requested, ASP.NET Boilerplate gets the text from the current language's XML
file (it finds the current language using
Thread.CurrentThread.**CurrentUICulture**). If it does not exists in the
current language, it gets the text from the default language's XML file.

##### Registering XML Localization Sources

XML files can be stored in the **file system** or can be **embedded** into
an assembly.

For **file system** stored XMLs, we can register the XML localization
source as shown below:

    Configuration.Localization.Sources.Add(
        new DictionaryBasedLocalizationSource(
            "SimpleTaskSystem",
            new XmlFileLocalizationDictionaryProvider(
                HttpContext.Current.Server.MapPath("~/Localization/SimpleTaskSystem")
                )
            )
        );

This is done in the **PreInitialize** event of a module (See the [module
system](/Pages/Documents/Module-System) for more info). ASP.NET
Boilerplate finds all the XML files in a given directory and registers the
localization source.

For **embedded XML files**, we must mark all localization XML files as an
**embedded resource** (Select XML files, open properties window (F4) and
change Build Action to Embedded Resource). We can then register the
localization source as shown below:

    Configuration.Localization.Sources.Add(
        new DictionaryBasedLocalizationSource(
            "SimpleTaskSystem",
            new XmlEmbeddedFileLocalizationDictionaryProvider(
                Assembly.GetExecutingAssembly(),
                "MyCompany.MyProject.Localization.Sources"
                )
            )
        );

**XmlEmbeddedFileLocalizationDictionaryProvider** gets an assembly
containing XML files (GetExecutingAssembly simply refers to current
assembly) and a **namespace** of XML files (namespace is the calculated
assembly name + folder hierarchy of XML files).

**Note**: When adding a language postfix to embedded XML files, **do not**
use the dot notation like 'MySource.tr.xml', instead use a dash like
'**MySource-tr.xml**' because dot notation causes namespacing problems
when finding resources!

#### JSON Files

JSON files can be used to store texts for a localization source. A
sample JSON localization file is shown below:

    {
      "culture": "en",
      "texts": {
        "TaskSystem": "Task system",
        "Xtasks": "{0} tasks"
      }
    }

JSON files should be unicode (**utf-8**). **culture: "en"** declares
that this JSON file contains English texts. We create a separate JSON
file for **each language** as shown below:

<img src="images/json-localization-source-files.png" alt="JSON localization files" class="img-thumbnail" />

**MySourceName** is the **source name** here, and MySourceName.json
defines the **default language**. It's similar to XML files.

##### Registering JSON Localization Sources

JSON files can be stored in the **file system** or can be **embedded** into
an assembly.

For file system stored JSONs, we can register a JSON localization
source as shown below:

    Configuration.Localization.Sources.Add(
        new DictionaryBasedLocalizationSource(
            "MySourceName",
            new JsonFileLocalizationDictionaryProvider(
                HttpContext.Current.Server.MapPath("~/Localization/MySourceName")
                )
            )
        );

This is done in **PreInitialize** event of a module (See the [module
system](/Pages/Documents/Module-System) for more info). ASP.NET
Boilerplate finds all JSON files in a given directory and registers the
localization source.

For **embedded JSON files**, we must mark all localization JSON files
as an **embedded resource** (Select JSON files, open properties window (F4)
and change Build Action as Embedded Resource). We can then register the
localization source as shown below:

     Configuration.Localization.Sources.Add(
        new DictionaryBasedLocalizationSource(
            "MySourceName",
            new JsonEmbeddedFileLocalizationDictionaryProvider(
                Assembly.GetExecutingAssembly(),
                "MyCompany.MyProject.Localization.Sources"
                )
            )
        );

**JsonEmbeddedFileLocalizationDictionaryProvider** gets an assembly
containing JSON files (GetExecutingAssembly simply refers to current
assembly) and a **namespace** of JSON files (namespace is the calculated
assembly name + folder hierarchy of JSON files).

Note: When adding a language postfix to embedded JSON files, **do not**
use the dot notation like 'MySource.tr.json'! Instead, use the dash like
'**MySource-tr.json**', since dot notation causes namespace problems when
finding resources. 

#### Resource Files

Localization text can also be stored in .NET's resource files. We can
create a resource file for each language as shown below (Right click
the project, choose add new item, then find resources file).

<img src="images/resource_files.png" alt="Localization resource files" class="img-thumbnail" />

**MyTexts.resx** contains the default language texts and
MyTexts**.tr**.resx contains texts for the Turkish language. When we open
MyTexts.resx, we can see all the texts:

<img src="images/resource_files_content.png" alt="Content of a resource file" class="img-thumbnail" />

In this case, ASP.NET Boilerplate uses .NET's built-in resource manager
for localization. You should configure a localization source for the
resource:

    Configuration.Localization.Sources.Add(
        new ResourceFileLocalizationSource(
            "MySource",
            MyTexts.ResourceManager
            ));

The **uniqe name** of the source is **MySource** here. And
**MyTexts.ResourceManager** is a reference to the resource manager that is
used to get localized texts. This is done in the **PreInitialize** event of
the module (See the [module system](/Pages/Documents/Module-System) for more
info).

#### Custom Source

A custom localization source can be implemented to store texts in
different sources such as in a database. You can directly implement the
**ILocalizationSource** interface or you can use the
**DictionaryBasedLocalizationSource** class to make implementation
easier (json and xml localization sources also use it). [Module
zero](Zero/Language-Management.md) implements the source in the database
for example.

### How the Current Language is Determined

#### ASP.NET Core

ASP.NET Core has its own mechanism to determine the current language.
Abp.AspNetCore package automatically adds ASP.NET Core's
**UseRequestLocalization** middleware to request pipeline. It also adds
some special providers. Here is the default ordered list of all providers,
which determine the current language for an HTTP request:

-   **QueryStringRequestCultureProvider** (ASP.NET Core's default
    provider): Uses **culture** & **ui-culture** URL query string values,
    if present. Example value: "culture=es-MX&ui-culture=es-MX".
-   **AbpUserRequestCultureProvider** (ABP's provider): If the user is known
    via **[IAbpSession](Abp-Session.md)** and has explicitly selected a
    language before (and saved to
    [ISettingManager](Setting-Management.md)), then use the user's
    preferred language. If the user is known but has not selected any language
    and the **.AspNetCore.Culture** cookie or header has a value, set the user's
    language setting with that information and use this value as the
    current language. If the user is unknown, this provider does nothing.
-   **AbpLocalizationHeaderRequestCultureProvider** (ABP's provider):
    Use **.AspNetCore.Culture** header value if present. Example value:
    "c=en|uic=en-US".
-   **CookieRequestCultureProvider** (ASP.NET Core's default provider):
    Use **.AspNetCore.Culture** cookie value if present. Example value:
    "c=en|uic=en-US".
-   **AbpDefaultRequestCultureProvider** (ABP's provider): If there is
    an default/application/tenant **setting value** for the language
    (named "Abp.Localization.DefaultLanguageName"), then use the
    setting's value.
-   **AcceptLanguageHeaderRequestCultureProvider** (ASP.NET Core's
    default provider): Use the **Accept-Language** header value if present
    (automatically sent by browsers). Example value:
    "tr-TR,tr;q=0.8,en-US;q=0.6,en;q=0.4".

The **UseRequestLocalization** middleware is automatically added when
you call the **app.UseAbp()** method. However, it's suggested that you manually add
it (in the Configure method of the Startup class) after the authentication
middleware if your application uses authentication. Otherwise, the
localization middleware does not know the current user to determine the best
language. Example usage:

    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
        app.UseAbp(options =>
        {
            options.UseAbpRequestLocalization = false; //disable automatic adding of request localization
        });
    
        //...authentication middleware(s)
    
        app.UseAbpRequestLocalization(); //manually add request localization
    
        //...other middlewares
    
        app.UseMvc(routes =>
        {
            //...
        });
    }

Most of time, you don't need to worry if you are using ABP's
localization system properly. See the ASP.NET Core [localization
document](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization)
to understand it better.

#### ASP.NET MVC 5.x

ABP automatically determines the current language in every web request and
sets the **current thread's culture (and UI culture)**. This is how ABP
determines it by default. ABP will:

-   Try to get it from a special **query string** value, named
    "**Abp.Localization.CultureName**" by default.
-   If the user is known via [IAbpSession](Abp-Session.md) and has explicitly
    selected a language before (and saved to
    [ISettingManager](Setting-Management.md)) then it uses the user's
    preferred language. If the user is known but has not selected any language,
    and the cookie/header (see below) has a value, it sets the user's setting with
    that information.
-   Try to get it from a special **header** value, named
    "**Abp.Localization.CultureName**" by default.
-   Try to get it from a special **header** value, named
    "**Abp.Localization.CultureName**" by default.
-   Try to get it from a special **cookie** value, named
    "**Abp.Localization.CultureName**" by default.
-   Try to get it from **default culture** setting (setting name is
    "Abp.Localization.DefaultLanguageName", which is a constant defined
    in Abp.Localization.LocalizationSettingNames.DefaultLanguage and can
    be changed using the [setting management](Setting-Management.md)).
-   Try to get it from the **browser's default language**
    (HttpContext.Request.UserLanguages).

If you need to, you can change the special cookie/header/querystring name
in your module's PreInitialize method. Example:

    Configuration.Modules.AbpWeb().Localization.CookieName = "YourCustomName";

ABP overrides **Application\_PostAuthenticateRequest** (in global.asax)
to implement that logic. You can override **SetCurrentCulture** in the
global.asax or replace **ICurrentCultureSetter** in order to override
the logic described above.

### Getting A Localized Text

After creating a source and registering it to the ASP.NET Boilerplate's
localization system, text can be localized easily. 

#### Server Side

On the server side, we can [inject](/Pages/Documents/Dependency-Injection)
**ILocalizationManager** and use it's **GetString** method.

    var s1 = _localizationManager.GetString("SimpleTaskSystem", "NewTask");

The GetString method gets the string from the localization source based on the
**current thread's UI culture**. If not found, it falls back to the **default
language**.

If a given string is not defined anywhere, then it returns the **given
string** by humanizing and wrapping it with **\[** and **\]** by default
(instead of throwing an Exception). Example: If a given text is
"ThisIsMyText", then the result will be "\[This is my text\]". This behavior
is configurable (you can use the Configuration.Localization in the PreInitialize method
of your [module](/Pages/Documents/Module-System) to change it).

Instead of always repeating your source name, you can first **get the source** and
then get a string from the source:

    var source = _localizationManager.GetSource("SimpleTaskSystem");
    var s1 = source.GetString("NewTask");

This returns the text in the current language. There are also overrides of
GetString to get the text in **different languages** and **formatted by
arguments**.

If we can not inject ILocalizationManager (maybe it's in a static context
that can not be reached by the dependency injection), we can simply use the
**LocalizationHelper** static class. We prefer injecting and using the
ILocalizationManager where it's possible since LocalizationHelper is
static and statics are difficult to test.

If you need localization in an [**application
service**](/Pages/Documents/Application-Services#applicationservice-class), in
an **MVC Controller**, in a **Razor View**, or in another class derived
from **AbpServiceBase**, there are shortcut **L** methods.

##### In MVC Controllers

Localization text is generally needed in an MVC Controller and Views.
There is a shortcut for that. See the sample controller below:

    public class HomeController : SimpleTaskSystemControllerBase
    {
        public ActionResult Index()
        {
            var helloWorldText = L("HelloWorld");
            return View();
        }
    }

The **L** method is used to localize a string. You must supply a
source name. It's done in the SimpleTaskSystemControllerBase as shown below:

    public abstract class SimpleTaskSystemControllerBase : AbpController
    {
        protected SimpleTaskSystemControllerBase()
        {
            LocalizationSourceName = "SimpleTaskSystem";
        }
    }

Note that it is derived from **AbpController** and therefore, you can easily localize text with the **L** method.

##### In MVC Views

The same **L** method also exists in views:

    <div>
        <form id="NewTaskForm" role="form">
            <div class="form-group">
                <label for="TaskDescription">@L("TaskDescription")</label>
                <textarea id="TaskDescription" data-bind="value: task.description" class="form-control" rows="3" placeholder="@L("EnterDescriptionHere")" required></textarea>
            </div>
            <div class="form-group">
                <label for="TaskAssignedPerson">@L("AssignTo")</label>
                <select id="TaskAssignedPerson" data-bind="options: people, optionsText: 'name', optionsValue: 'id', value: task.assignedPersonId, optionsCaption: '@L("SelectPerson")'" class="form-control"></select>
            </div>
            <button data-bind="click: saveTask" type="submit" class="btn btn-primary">@L("CreateTheTask")</button>
        </form>
    </div>

To make this work, you should derive your views from a base class that
sets the source name:

    public abstract class SimpleTaskSystemWebViewPageBase : SimpleTaskSystemWebViewPageBase<dynamic>
    {
    
    }
    
    public abstract class SimpleTaskSystemWebViewPageBase<TModel> : AbpWebViewPage<TModel>
    {
        protected SimpleTaskSystemWebViewPageBase()
        {
            LocalizationSourceName = "SimpleTaskSystem";
        }
    }

Then set this view base class in web.config:

    <pages pageBaseType="SimpleTaskSystem.Web.Views.SimpleTaskSystemWebViewPageBase">

All controllers and views are ready with these methods when you create your
solution from one of the ASP.NET Boilerplate [templates](/Templates).

#### In JavaScript

ASP.NET Boilerplate also makes it possible to use the same localization text in
JavaScript. First, you need to add the dynamic ABP scripts to
the page:

    <script src="/AbpScripts/GetScripts" type="text/javascript"></script>

ASP.NET Boilerplate automatically generates the needed JavaScript code to
get localized text on the client side. You can then easily get a
localized text in JavaScript as shown below:

    var s1 = abp.localization.localize('NewTask', 'SimpleTaskSystem');

NewTask is the text name and SimpleTaskSystem is the source name.
Instead of repeating the source name each time, you can first get the source and then get the
text:

    var source = abp.localization.getSource('SimpleTaskSystem');
    var s1 = source('NewTask');

##### Format Arguments

The localization method can also get additional format arguments. Example:

    abp.localization.localize('RoleDeleteWarningMessage', 'MySource', 'Admin');
    
    //shortcut if the source is retrieved using getSource as shown above
    source('RoleDeleteWarningMessage', 'Admin');

if RoleDeleteWarningMessage = 'Role {0} will be deleted', then the localized
text will be 'Role Admin will be deleted'.

##### Default Localization Source

You can set a default localization source and use the
abp.localization.localize method without the source name.

    abp.localization.defaultSourceName = 'SimpleTaskSystem';
    var s1 = abp.localization.localize('NewTask');

defaultSourceName is global and works for only one source at a time.

### Extending Localization Sources

Assume that we use a module which defines its own localization source.
We may need to change it's localized texts, add new text or translate
to other languages. ASP.NET Boilerplate allows for extending a localization
source. It currently works for XML and JSON files (Actually any
localization source that implements the IDictionaryBasedLocalizationSource
interface).

ASP.NET Boilerplate also defines some localization sources. For
instance, the **Abp.Web** NuGet package defines a localization source named
"**AbpWeb**" as embedded XML files:

<img src="images/AbpWeb-localization-source.png" alt="AbpWeb localization source files" class="img-thumbnail" />

The default (English) XML file looks like this (only the first two texts are
shown):

    <?xml version="1.0" encoding="utf-8" ?>
    <localizationDictionary culture="en">
      <texts>
        <text name="InternalServerError" value="An internal error occurred during your request!" />
        <text name="ValidationError" value="Your request is not valid!" />
        ...
      </texts>
    </localizationDictionary>

To extend AbpWeb source, we can define XML files. Assume that we only
want to change the **InternalServerError** text. We can define an XML file
as shown below:

    <?xml version="1.0" encoding="utf-8" ?>
    <localizationDictionary culture="en">
      <texts>
        <text name="InternalServerError" value="Sorry :( It seems there is a problem. Let us to solve it and please try again later." />
      </texts>
    </localizationDictionary>

We can then register it on the PreInitialize method of our module:

    Configuration.Localization.Sources.Extensions.Add(
    	new LocalizationSourceExtensionInfo("AbpWeb",
    		new XmlEmbeddedFileLocalizationDictionaryProvider(
    			Assembly.GetExecutingAssembly(),
    			"MyCompany.MyProject.Localization.Sources"
    		)
    	)
    );

ASP.NET Boilerplate overrides (merges) the base localization source with our
XML files. We can also add new language files.

**Note**: We can use JSON files to extend XML files, or vice verse. The files created for extending localization sources must be marked as **embedded resource**.

### Getting Languages

ILanguageManager can be used to get a list of all available languages
and the current language.

### Best Practices

XML files, JSON files and Resource files have their own strengths and
weaknesses. We suggest you use XML or JSON files instead of Resource
files, because;

-   XML/JSON files are easy to edit, extend or port.
-   XML/JSON files require string keys while getting localized texts
    instead of compile-time properties like Resource files. This can be
    considered as a weakness. However, it's easier to change the source later.
    We can even move the localization to a database without changing the code
    which uses localization (**Module Zero** implements it to create a
    **database based** and **per-tenant** localization source. See
    [documentation](/Pages/Documents/Zero/Language-Management).)

If you use XML or JSON, we recommend you do not sort the texts by name.
Sort them by creation date! This way, when someone translates it to another
language, he/she can easily see which texts have been added recently.
