### Introduction

While some applications target a single timezone, others target
many different timezones. To satisfy such needs and centralize datetime
operations, ABP provides a common infrastructure for datetime operations.

### Clock

**Clock** is the main class used to work with **DateTime** values. It
defines the following **static** properties and methods:

-   **Now**: Gets the current time according to the current provider.
-   **Kind**: Gets the DateTimeKind of the current provider.
-   **SupportsMultipleTimezone**: Gets a value that indicates whether or not a current
    provider can be used for applications that need multiple timezones.
-   **Normalize**: Normalizes/converts a given DateTime using the current
    provider.

So, instead of using *DateTime.Now*, we use **Clock.Now**, which
abstracts it:

    DateTime now = Clock.Now;

Clock uses the clock providers inside it. There are three types of
**built-in clock providers**:

-   **ClockProviders.Unspecified** (UnspecifiedClockProvider): This is
    the **default** clock provider and behaves just like
    **DateTime.Now**. It acts as if you don't use the Clock class at all.
-   **ClockProviders.Utc** (UtcClockProvider): Works in UTC datetime.
    **DateTime.UtcNow** for Clock.Now. The Normalize method converts a given
    datetime to a utc datetime and then sets it's kind to DateTimeKind.UTC. It
    **supports multiple timezones**.
-   **ClockProviders.Local** (LocalClockProvider): Works in the local
    computer's time. The Normalize method converts a given datetime to a local
    datetime and sets it's kind to DateTimeKind.Local.

You can set Clock.Provider in order to use a different clock provider:

    Clock.Provider = ClockProviders.Utc;

This is generally done at the beginning of an application (do
it in the Application\_Start of a web application).

#### Client-Side

The clock can be used on the client-side using the **abp.clock** object in
JavaScript. When you set Clock.Provider on the server-side, ABP
automatically sets the value of **abp.clock.provider** on the client-side.

### Time Zones

ABP defines a [setting](Setting-Management.md) named
**Abp.Timing.TimeZone** (*TimingSettingNames.TimeZone* constant) for
storing the selected timezone of the host, tenant and user. ABP assumes that
the value of a timezone setting is a valid **Windows timezone name**. It also
defines a timezone mapping file to convert a Windows Timezone to an
**IANA** timezone since some common libraries are using the IANA
timezone id. **UtcClockProvider** must be used in order to support
**multiple timezones**. Because if UtcClockProvider is used, all
datetime values will be stored in UTC and all datetimes will be sent to
clients in UTC format. Then on the client-side we can convert a UTC
datetime to the user's timezone by using the user's current timezone setting. 

#### Client-Side

ABP creates a JavaScript object named **abp.timing.timeZoneInfo** which
contains timezone information for the current user. This information
contains Windows and IANA timezone ids and some extra information for
windows timezone info. This information can be used to make client-side
datetime convertions by showing a datetime to the user in his/her timezone.

### Binders and Converters

-   ABP automatically normalizes DateTimes received (in an input class) from **clients** in
    MVC, Web API and ASP.NET Core applications, based on the current
    clock provider.
-   ABP automatically normalizes DateTimes received from the **database**
    based on the current clock provider, when
    [EntityFramework](EntityFramework-Integration.md) or
    [NHibernate](NHibernate-Integration.md) modules are used.

If the UTC clock provider is used, then all DateTimes stored in the database are
assumed as UTC values, and all DateTimes received from clients are assumed
as UTC values unless explicitly specified.
