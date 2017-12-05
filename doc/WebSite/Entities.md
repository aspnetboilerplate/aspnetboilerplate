Entities are one of the core concepts of DDD (Domain Driven Design).
Eric Evans describe it as "*An object that is not fundamentally defined
by its attributes, but rather by a thread of continuity and identity*".
So, entities have Id's and stored in a database. An entity is generally
mapped to a table for relational databases.

### Entity Class

In ASP.NET Boilerplate, Entities are derived from **Entity** class. See
the sample below:

    public class Person : Entity
    {
        public virtual string Name { get; set; }

        public virtual DateTime CreationTime { get; set; }

        public Person()
        {
            CreationTime = DateTime.Now;
        }
    }

**Person** class is defined as an entity. It has two properties. Also,
Entity class defines an **Id** property. It's **primary key** of the
Entity. So, name of primary keys of all Entities are same, it's **Id**.

Type of Id (primary key) can be changed. It's **int** (Int32) by
default. If you want to define another type as Id, you should explicitly
declare it as shown below:

    public class Person : Entity<long>
    {
        public virtual string Name { get; set; }

        public virtual DateTime CreationTime { get; set; }

        public Person()
        {
            CreationTime = DateTime.Now;
        }
    }

Also, you can set it as string, Guid or something else.

Entity class overrides **equality** operator (==) to easily check if two
entities are equal (their Id is equal). It also defines the
**IsTransient()** method to check if it has an Id or not.

### AggregateRoot Class

"*Aggregate is a pattern in Domain-Driven Design. A DDD aggregate is a
cluster of domain objects that can be treated as a single unit. An
example may be an order and its line-items, these will be separate
objects, but it's useful to treat the order (together with its line
items) as a single aggregate.*" (Martin Fowler - see [full
description](http://martinfowler.com/bliki/DDD_Aggregate.html))

While ABP does not enforce you to use aggregates, you may want to create
aggregates and aggregate roots in your application. ABP defines
**AggregateRoot** class that extends Entity to create aggregate root
entities for an aggregate.

#### Domain Events

AggregateRoot defines [**DomainEvents**](EventBus-Domain-Events.html)
collection to generate domain events by the aggregate root class. These
events are automatically triggered just before the current [unit of
work](Unit-Of-Work.html) is completed. Actually, any entity can generate
domain events by implementing **IGeneratesDomainEvents** interface, but
it's common (best practice) to generate domain events in aggregate
roots. That's why it's default for AggregateRoot but not for Entity
class.

### Conventional Interfaces

In many application, similar entity properties (and database table
fields) are used like CreationTime indicates that when this entity is
created. ASP.NET Boilerplate provides some useful interfaces to make
this common properties explicit and expressive. Also, this provides a
way of coding common code for Entities which implement these interfaces.

#### Auditing

**IHasCreationTime** makes it possible to use a common property for
'**creation time**' information of an entity. ASP.NET Boilerplate
automatically sets CreationTime to **current time** when an Entity is
inserted into database which implements this interface.

    public interface IHasCreationTime
    {
        DateTime CreationTime { get; set; }
    }

Person class can be re-written as shown below by implementing
IHasCreationTime interface:

    public class Person : Entity<long>, IHasCreationTime
    {
        public virtual string Name { get; set; }

        public virtual DateTime CreationTime { get; set; }

        public Person()
        {
            CreationTime = DateTime.Now;
        }
    }

**ICreationAudited** extens IHasCreationTime by adding 
**CreatorUserId**:

    public interface ICreationAudited : IHasCreationTime
    {
        long? CreatorUserId { get; set; }
    }

ASP.NET Boilerplate automatically sets CreatorUserId to **current user's
id** when saving a new entity. You can also implement ICreationAudited
easily by deriving your entity from **CreationAuditedEntity** class. It
has also a generic version for different type of Id properties.

There is also similar interfaces for modifications:

    public interface IHasModificationTime
    {
        DateTime? LastModificationTime { get; set; }
    }

    public interface IModificationAudited : IHasModificationTime
    {
        long? LastModifierUserId { get; set; }
    }

ASP.NET Boilerplate also automatically sets these properties when
updating an entity. You just define them for your entity.

If you want to implement all of audit properties, you can direcly
implement **IAudited** interface:

    public interface IAudited : ICreationAudited, IModificationAudited
    {

    }

As a shortcut, you can derive from **AuditedEntity** class instead of
direcly implementing **IAudited**. AuditedEntity class has also a
generic version for different type of Id properties.

Note: ASP.NET Boilerplate gets current user's Id from [ABP
Session](/Pages/Documents/Abp-Session).

#### Soft Delete

Soft delete is a commonly used pattern to mark an Entity as deleted
instead of actually deleting it from database. For instace, you may not
want to hard delete a User from database since it has many releations to
other tables. **ISoftDelete** interface is used for this purpose:

    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
    }

ASP.NET Boilerplate implements soft delete pattern out-of-the-box. When
a soft-delete entity is being deleted, ASP.NET Boilerplate detects this,
prevents deleting, sets IsDeleted as true and updates entity in the
database. Also, it does not retrive (select) soft deleted entities from
database, automatically filters them.

If you use soft delete, you may also want to store information when an
entity is deleted and who deleted it. You can implement
**IDeletionAudited** interface that is shown below:

    public interface IDeletionAudited : ISoftDelete
    {
        long? DeleterUserId { get; set; }

        DateTime? DeletionTime { get; set; }
    }
                

IDeletionAudited extends ISoftDelete as you noticed. ASP.NET Boilerplate
automatically sets these properties when an entity is deleted.

If you want to implement all audit interfaces (creation, modification
and deletion) for an entity, you can directly implement **IFullAudited**
since it inherits all:

    public interface IFullAudited : IAudited, IDeletionAudited
    {

    }

As a shortcut, you can derive your entity from **FullAuditedEntity**
class that implements all.

-   NOTE 1: All audit interfaces and classes have a generic version for
    defining navigation property to your **User** entity (like
    ICreationAudited&lt;TUser&gt; and FullAuditedEntity&lt;TPrimaryKey,
    TUser&gt;).
-   NOTE 2: Also, all of them has an **AggregateRoot** version, like
    AuditedAggregateRoot.

#### Active/Passive Entities

Some entities need to be marked as Active or Passive. Then you may take
action upon active/passive state of the entity. You can implement
**IPassivable** interface that is created for this reason. It defines
**IsActive** property.

If your entity will be active on first creation, you can set IsActive to
true in the constructor.

This is different than soft delete (IsDeleted). If an entity is soft
deleted, it can not be retrieved from database (ABP prevents it as
default). But, for active/passive entities, it's completely up to you to
control getting entities.

### Entity Change Events

ASP.NET Boilerplate automatically triggers certain events when an entity
is inserted, updated or deleted. Thus, you can register to these events
and perform any logic you need. See Predefined Events section in [event
bus documentation](/Pages/Documents/EventBus-Domain-Events) for more
information.

### IEntity Interfaces

Actually, **Entity** class implements **IEntity** interface (and
**Entity&lt;TPrimaryKey&gt;** implements
**IEntity&lt;TPrimaryKey&gt;**). If you do not want to derive from
Entity class, you can implement these interfaces directly. There are
also corresponding interfaces for other entity classes. But this is not
the suggested way, unless you have a good reason to do not derive from
Entity classes.

### IExtendableObject Interface

ASP.NET Boilerplate provides a simple interface, **IExtendableObject**,
to easily associate **arbitrary name-value data** to an entity. Consider
this simple entity:

    public class Person : Entity, IExtendableObject
    {
        public string Name { get; set; }

        public string ExtensionData { get; set; }

        public Person(string name)
        {
            Name = name;
        }
    }

**IExtendableObject** just defines **ExtensionData** string property
which is used to store **JSON formatted** name value objects. Example:

    var person = new Person("John");

    person.SetData("RandomValue", RandomHelper.GetRandom(1, 1000));
    person.SetData("CustomData", new MyCustomObject { Value1 = 42, Value2 = "forty-two" });

We can use any type of object as value to **SetData** method. When we
use such the code above, **ExtensionData** will be like that:

    {"CustomData":{"Value1":42,"Value2":"forty-two"},"RandomValue":178}

Then we can use **GetData** to get any value:

    var randomValue = person.GetData<int>("RandomValue");
    var customData = person.GetData<MyCustomObject>("CustomData");

While this technique can be very useful in some cases (when you need to
provide ability to dynamically add extra data to an entity), you
normally should use regular properties. Such a dynamic usage is not type
safe and explicit.
