### Introduction

ASP.NET Boilerplate provides an infrastructure to automatically log all
entity and property change history.

The saved fields for an entity change are: Related **tenant id**,
changer **user id**, **entity change set id**, **entity id**,
**entity type name**, **change time**, **change type**, the client's
**IP address**, the client's **computer name** and the **browser info** (if
entity is changed in a web request).

The saved fields for an entity property change are: Related **tenant id**,
**entity change id**, **property name**, **property type name**,
**new value** and the **original value**.

The entity changes are grouped in a change set for each SaveChanges call.

The Entity History tracking system uses
[**IAbpSession**](/Pages/Documents/Abp-Session) to
get the current UserId and TenantId.

No entities are automatically tracked by default. You should configure entities either by startup configuration or using attributes.

#### About IEntityHistoryStore

The Entity History tracking system uses **IEntityHistoryStore** to
save change information. While you can implement it in your own way,
it's fully implemented in the **module-zero** project.

### Configuration

To configure Entity History, you can use the
**Configuration.EntityHistory** property
in your [module](/Pages/Documents/Module-System)'s PreInitialize method.
Entity History is **enabled by default**.
You can disable it as shown below:

    public class MyModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.EntityHistory.IsEnabled = false;
        }

        //...
    }

Here are the Entity History configuration properties:

-   **IsEnabled**: Used to enable/disable the tracking system completely.
    Default: **true**.
-   **IsEnabledForAnonymousUsers**: If this is set to true, change logs
    are saved for users that are not logged in to the application.
    Default: **false**.
-   **Selectors**: Used to select entities to save change logs.

**Selectors** is a list of predicates to select entities to save
change logs. A selector has a unique **name** and a **predicate**.
For example, a selector can be used to select **full audited entities**.
It's defined as shown below:

    Configuration.EntityHistory.Selectors.Add(
        new NamedTypeSelector(
            "Abp.FullAuditedEntities",
            type => typeof (IFullAudited).IsAssignableFrom(type)
        )
    );

You can add your selectors in your module's PreInitialize method.

### Enable/Disable by attributes

While you can select tracked entities by configuration, you can use the
**HistoryTracked** and **DisableHistoryTracking** attributes for a single
**entity** or an individual **property**. Example:

    [HistoryTracked]
    public class MyEntity : Entity
    {
        public string MyProperty1 { get; set; }

        [DisableHistoryTracking]
        public int MyProperty2 { get; set; }

        public long MyProperty3 { get; set; }
    }

All properties of MyEntity are tracked except MyProperty2 since it's
explicitly disabled. The HistoryTracked attribute can be used to
save change logs for a desired property.

**DisableHistoryTracking** can be used for an entity or a single **property of an
entity**. Thus, you can **hide sensitive data** in change logs, such as
passwords for example.

### Notes

-   A property must be **public** in order to be saved in change logs.
    Private and protected properties are ignored.
-   DisableHistoryTracking takes priority over HistoryTracking attribute.
-   Only works for entities.
-   Only works for scalar properties, e.g. string, int, bool...
