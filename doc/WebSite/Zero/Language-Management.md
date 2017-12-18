### Introduction

ASP.NET Boilerplate defines a strong UI [localization
system](/Pages/Documents/Localization) which is used both in server and
client side. It allows to easily configure application languages and
define localization texts (strings) in different sources (Resource files
and XML files are two pre-defined source).

While it's good for most cases, we may want to define languages and
texts **dynamically** on a **database**. Module-Zero allows us to
dynamically manage application **languages** and **texts** **per
tenant**.

#### About Localization

It's strongly suggested to read [localization
documentation](/Pages/Documents/Localization) before this document.

### How To Enable?

#### Startup Template

If you create your project from [startup templates](/Templates), you can
skip this section since the template comes with database based
localization enabled by default. If you created your project before this
feature, please read this to enable it for your application.

Database localization is designed to be backward-compatible with ASP.NET
Boilerplate's existing localization system. It actually replaces all
existing dictionary based localization sources with the
**MultiTenantLocalizationSource**.

MultiTenantLocalizationSource wraps existing
**DictionaryBasedLocalizationSource** based sources. So, we generally
wrap [XML based
localization](/Pages/Documents/Localization#xml-files) sources. It
can not wrap **Resource File** sources since resource files are designed
to work hard-coded and static files which is not proper for dynamic
localization.

Since it's a wrapper, underlying XML files are used as fallback source
if a text is not localized in the database. It may seems complicated,
but easy to implement for your application. Let's see how to enable
database based localization.

#### EnableDbLocalization

First, we are enabling it:

    Configuration.Modules.Zero().LanguageManagement.EnableDbLocalization();

This should be done in the **top level** module's
**[PreInitialize](/Pages/Documents/Module-System#preinitialize)**
method (it's the web module for a web application. Import
Abp.Zero.Configuration namespace (using Abp.Zero.Configuration) to see
the Zero() extension method).

Actually, this configuration makes all the magic. But, we should make a
bit more to make it properly work.

#### Seed Database Languages

Since ABP will get list of languages from database anymore, we should
**insert** default languages to database. If you're using
EntityFramework, you can use seed code as [like this
one](https://github.com/aspnetboilerplate/module-zero-template/blob/master/src/AbpCompanyName.AbpProjectName.EntityFramework/Migrations/SeedData/DefaultLanguagesCreator.cs):

#### Remove Static Language Configuration

If you have static language configuration as shown below, you can
**delete** these lines from your configuration code, since it will get
languages from database anymore.

    Configuration.Localization.Languages.Add(new LanguageInfo("en", "English", "famfamfam-flag-england", true));

#### Note On Existing XML Localization Sources

**Do not** delete your XML localization files and source configuration
code. Because, these files are used as **fallback source** and also all
localization keys are obtained from this source.

So, when you need to a new localized text, **define it** into XML files
as you do normally. You should at least define it in **default**
language's XML file. Also thus, you don't need to add default values of
localized texts to database migration code.

### Managing Languages

**IApplicationLanguageManager** interface is
[injected](/Pages/Documents/Dependency-Injection) and used to manage
languages. It has methods like GetLanguagesAsync, AddAsync, RemoveAsync,
UpdateAsync... to manage languages for host and tenants.

#### Language List Logic

List of languages are stored per tenant and for the host, and calculated
as below:

-   There is a list of languages for defined for **the host**. This list
    is considered as **default** for all tenants.
-   There is a seperated list of languages for **each tenant**. This
    list **inherits** the host list **adds** tenant-specific languages.
    Tenants can not delete or update host-defined (default) languages
    (but can override localization texts as we will see later).

#### ApplicationLanguage Entity

ApplicationLanguage entity represents a language for a tenant or the
host.

    [Serializable]
    [Table("AbpLanguages")]
    public class ApplicationLanguage : FullAuditedEntity, IMayHaveTenant
    {
        //...
    }

It's basic properties are:

-   **TenantId** (nullable): Contains the related tenant's Id if this
    language is tenant-specific. It's null if this is a host language.
-   **Name**: Name of the language. This **must be a culture code** from
    the list
    [here](https://msdn.microsoft.com/en-us/library/ee825488(v=cs.20).aspx).
-   **DisplayName**: Shown name of the language. This can be an
    arbitrary name, generally is
    [CultureInfo.DisplayName](https://msdn.microsoft.com/en-us/library/system.globalization.cultureinfo.displayname(v=vs.110).aspx).
-   **Icon**: An arbitrary icon/flag for the language. This can be used
    to show flag of the language on UI.

Also, ApplicationLanguage inherits **FullAuditedEntity** as you see.
That means, it's a **soft-delete** entity and automatically **audited**
(see [entity document](/Pages/Documents/Entities) for more).

ApplicationLanguage entities are stored in **AbpLanguages** table in the
database.

### Managing Localization Texts

**IApplicationLanguageTextManager** interface is
[injected](/Pages/Documents/Dependency-Injection) and used to manage
localization texts. It has needed methods to get/set a localization text
for a tenant or the host.

#### Localizing A Text

Let's see what happens when you want to localize a text;

-   Try to get for the **current culture** (got using
    CurrentThread.CurrentUICulture).
    -   It checks if given text is defined (overrided) for **current
        tenant** (got using
        [IAbpSession.TenantId](/Pages/Documents/Abp-Session)) in
        **current culture** in the database. Returns the value if
        defined.
    -   It then checks if given text is defined (overrided) for the
        **host** in **current culture** in the database. Returns the
        value if defined.
    -   It then checks if given text is defined in the underlying XML
        file in **current culture**. Returns the value if defined.
-   Try to find for the **fallback culture**. It's calculated like that:
    If current culture is "en-GB" then fallback culture is "en".
    -   It checks if given text is defined (overrided) for **current
        tenant** in **fallback culture** in the database. Returns the
        value if defined.
    -   It then checks if given text is defined (overrided) for the
        **host** in **fallback culture** in the database. Returns the
        value if defined.
    -   It then checks if given text is defined in the underlying XML
        file in **fallback culture**. Returns the value if defined.
-   Try to find for the **default culture**.
    -   It checks if given text is defined (overrided) for **current
        tenant** in **default culture** in the database. Returns the
        value if defined.
    -   It then checks if given text is defined (overrided) for the
        **host** in **default culture** in the database. Returns the
        value if defined.
    -   It then checks if given text is defined in the underlying XML
        file in **default culture**. Returns the value if defined.
-   Get the same text or throw exception
    -   If given text (key) is not found at all, ABP throws exception or
        returns the same text (key) by wrapping with \[ and \] (it can
        be configured on startup, see [localization
        document](/Pages/Documents/Localization)).

So, getting a localized text is a bit complicated. But it works fast
since it uses [cache](/Pages/Documents/Caching).

#### ApplicationLanguageText Entity

ApplicationLanguageText is used to store localized values in the
database.

    [Serializable]
    [Table("AbpLanguageTexts")]
    public class ApplicationLanguageText : AuditedEntity<long>, IMayHaveTenant
    {
        //...
    }

It's basic properties are;

-   **TenantId** (nullable): Contains the related tenant's Id if this
    localized text is tenant-specific. It's null if this is a
    host-localized text.
-   **LanguageName**: Name of the language. This **must be a culture
    code** from the list
    [here](https://msdn.microsoft.com/en-us/library/ee825488(v=cs.20).aspx).
    This matches to ApplicationLanguage.Name but not forced a foreign
    key to make it independent from the language entry. 
    IApplicationLanguageTextManager handles it properly.
-   **Source**: Localization source name.
-   **Key**: Localization text's key/name.
-   **Value**: Localized value.

ApplicationLanguageText entities are stored in **AbpLanguageTexts**
table in the database.
