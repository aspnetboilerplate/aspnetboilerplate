## Introduction

ASP.NET Boilerplate provides an infrastructure to automatically log all
entity and property changes.

The saved fields for an entity change are: The related **tenant id**,
**entity change set id**, **entity id**,
**entity type name**, **change time** and the **change type**.

The saved fields for an entity property change are: The related **tenant id**,
**entity change id**, **property name**, **property type name**,
**new value** and the **original value**.

The entity changes are grouped in a change set for each SaveChanges call.

The saved fields for an entity change set are: The related **tenant id**,
changer **user id**, **creation time**, **reason**, the client's
**IP address**, the client's **computer name** and the **browser info** (if
entities are changed in a web request).

The Entity History tracking system uses
[**IAbpSession**](/Pages/Documents/Abp-Session) to
get the current UserId and TenantId.

No entities are automatically tracked by default. You should configure entities either by using the startup configuration or via attributes.

## About IEntityHistoryStore

The Entity History tracking system uses **IEntityHistoryStore** to
save change information. While you can implement it in your own way,
it's fully implemented in the **Module Zero** project.

## Configuration

To configure Entity History, you can use the
**Configuration.EntityHistory** property
in your [module](/Pages/Documents/Module-System)'s PreInitialize method.
Entity History is **enabled by default**.
You can disable it as shown below:

```csharp
    public class MyModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.EntityHistory.IsEnabled = false;
        }

        //...
    }
```

Here are the Entity History configuration properties:

-   **IsEnabled**: Used to enable/disable the tracking system completely.
    Default: **true**.
-   **IsEnabledForAnonymousUsers**: If this is set to true, the change logs
    are saved for users that are not logged in to the application.
    Default: **false**.
-   **Selectors**: Used to select entities to save change logs.
-   **IgnoredTypes**: Used to skip entities when saving change logs.

**Selectors** is a list of predicates to select entities to save
change logs. A selector has a unique **name** and a **predicate**.
For example, a selector can be used to select **full audited entities**.
It's defined as shown below:

```csharp
    Configuration.EntityHistory.Selectors.Add(
        new NamedTypeSelector(
            "Abp.FullAuditedEntities",
            type => typeof (IFullAudited).IsAssignableFrom(type)
        )
    );
```

You can add your selectors in your module's PreInitialize method.

**IgnoredTypes** is a list of entity types to be ignored when saving
change logs.
For example, we configured all entities that implements **ISetting** to be tracked by the Entity History system.
In order to skip entity history for **SecretSetting** which implements **ISetting**.
It can be defined as shown below:

```csharp
    Configuration.EntityHistory.IgnoredTypes.AddIfNotContains(typeof(SecretSetting));
```

You can add your ignored types in your module's PreInitialize method.

#### Notes
-   **IgnoredTypes** takes priority over the **Selectors** when saving change log for the entities.
-   **EntityChangeSet**, **EntityChange**, **EntityPropertyChange** are added to **IgnoredTypes** by **default** in the Entity History system.
-   **AuditLog** is added to **IgnoredTypes** by **default** in Module Zero project.

### Enable/Disable by attributes

While you can select tracked entities by configuration, you can use the
**Audited** and **DisableAuditing** attributes for a single
**entity** or an individual **property**. Example:

```csharp
    [Audited]
    public class MyEntity : Entity
    {
        public string MyProperty1 { get; set; }

        [DisableAuditing]
        public int MyProperty2 { get; set; }

        public long MyProperty3 { get; set; }
    }
```

All properties of MyEntity are tracked except MyProperty2 since it's
explicitly disabled. The Audited attribute can be used to
save change logs for a desired property.

**DisableAuditing** can be used for an entity or a single **property of an
entity**. Thus, you can **hide sensitive data** in change logs, such as
passwords for example.

### Reason Property

The entity change set has a **Reason** property that can be used to understand why a
set of changes has occurred, i.e. the use case that resulted in these changes.

For example, Person A transfers money from Account A to Account B. Both account
balances change and "Money transfer" is recorded as the Reason for this change set.
Since a balance change can be due to other reasons, the Reason property explains
why these changes were made.

The **Abp.AspNetCore** package implements **HttpRequestEntityChangeSetReasonProvider**,
which returns the HttpContext.Request's URL as the Reason.

### UseCase Attribute

The preferred approach is using the **UseCase** attribute. Example:

```csharp
    [UseCase(Description = "Assign an issue to a user")]
    public virtual async Task AssignIssueAsync(AssignIssueInput input)
    {
        // ...

        await _unitOfWorkManager.Current.SaveChangesAsync();
    }
```

#### UseCase Attribute Restrictions

You can use the UseCase attribute for:

-   All **public** or **public virtual** methods for classes that are
    used via its interface, e.g. an application service used via its interface.
-   All **public virtual** methods for self-injected classes, e.g. **MVC
    Controllers**.
-   All **protected virtual** methods.

### IEntityChangeSetReasonProvider

In some cases, you may need to change/override the Reason value for a limited scope.
You can use the **IEntityChangeSetReasonProvider.Use(...)** method as shown below:

```csharp
    public class MyService
    {
        private readonly IEntityChangeSetReasonProvider _reasonProvider;

        public MyService(IEntityChangeSetReasonProvider reasonProvider)
        {
            _reasonProvider = reasonProvider;
        }

        public virtual async Task AssignIssueAsync(AssignIssueInput input)
        {
            var reason = "Assign an issue to user: " + input.UserId.ToString();
            using (_reasonProvider.Use(reason))
            {
                ...

                await _unitOfWorkManager.Current.SaveChangesAsync();
            }
        }
    }
```

The Use method returns an IDisposable and it **must be disposed**. Once the return
value is disposed, the Reason is automatically restored to the previous value.


### IEntitySnapshotManager

You may need to get a snapshot of your entity on a given date. You can use the `IEntitySnapshotManager.GetSnapshotAsync(...)` method as shown below:

```csharp
    public class MyService
    {
        private readonly IEntitySnapshotManager _entitySnapshotManager;

        public MyService(IEntitySnapshotManager entitySnapshotManager)
        {
            _entitySnapshotManager = entitySnapshotManager;
        }

        public Task<EntityHistorySnapshot> GetMyEntitySnapshot(long id, DateTime time)
        {
            return _entitySnapshotManager.GetSnapshotAsync<MyEntity, long>(id, time);
        }
      
        public async Task<string> GetMyEntityMyPropertySnapshotValue(long id, DateTime time)
        {
           var snapshot = await GetMyEntitySnapshot(id, time);
           if (snapshot.IsPropertyChanged(nameof(MyEntity.MyProperty)))
           {
               // var stacktree = snapshot.PropertyChangesStackTree[nameof(MyEntity.MyProperty)];
               return snapshot[nameof(MyEntity.MyProperty)];               
           }
           return null;
        }
    }
```

`IEntitySnapshotManager.GetSnapshotAsync(...)` returns an `EntityHistorySnapshot` that contains the snapshot value of all changed properties on the given date and a stack tree of the changes.


## ORM Integrations

Entity History in **Module Zero** project is implemented for
[Entity Framework](EntityFramework-Integration.md) 6.x and
[Entity Framework Core](Entity-Framework-Core.md) with the following logics:

### Entity Changes

-   An entity must not be one of the **IgnoredTypes**.
-   An entity must be **public** in order to be saved in the change logs.
    **private**, **protected**, **internal** entities are ignored.
-   **DisableAuditing** takes priority over the **Audited** attribute when saving entity changes.
-   An entity must satisfy one of the predicates from **Selectors**.

### Property Changes

-   Primary key properties are excluded from an entitiy's property changes.
-   **DisableAuditing** takes priority over the **Audited** attribute when saving property changes.
-   Property changes would only be saved if there are changes between original/new values.
    **Audited** attribute will make property changes being saved even if there is no change between original/new values.

## Entity Framework Core

-   Entity changes only work for entities and owned entities.
-   Property changes only work for scalar properties (e.g. string, int, bool...)
    -   including shadow properties.

### Owned Entities

Entity History tracks changes to owned entities as entity changes.
The primary key of the owner entity is saved as the **entity id**.

## Entity Framework 6.x

-   Entity changes only work for entities.
-   Property changes only work for complex type properties and scalar properties (e.g. string, int, bool...)
    -   including relationship changes.

### Complex Type Properties

Entity History tracks changes to complex type properties as property changes (unlike owned entities for EF Core).
The original/new values of a complex type property will be a serialized JSON string of the complex type property.

### Relationship Changes

Entity History tracks changes to relationships (navigation properties without foreign key) as property changes (like shadow properties for EF Core).

However, these use the navigation property name (e.g. Blog) as declared in the entity, unlike the shadow property name (e.g. BlogId) generated by EF Core.
