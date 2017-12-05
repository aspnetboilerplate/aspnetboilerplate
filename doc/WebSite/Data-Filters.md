### Introduction

It's common to use the
[**soft-delete**](/Pages/Documents/Entities#DocSoftDelete) pattern which
is used to not delete an entity from database but only mark it as
'deleted'. So, if an entity is soft-deleted, it should not be accidently
retrieved into the application. To provide that, we would add a SQL
**where** condition like 'IsDeleted = false' in every query we select
entities. This is a tedious but more importantly a forgettable task. So,
there should be an automatic way of it.

ASP.NET Boilerplate provides **data filters** those can be used to
automatically filter queries based on some rules. There are some
pre-defined filters, but also you can create your own filters.

### Pre-Defined Filters

#### ISoftDelete

Soft-delete filter is used to automatically filter (extract from
results) deleted entities while querying database. If an
[entity](/Pages/Documents/Entities) should be soft-deleted, it must
implement **ISoftDelete** interface which defines only **IsDeleted**
property. Example:

    public class Person : Entity, ISoftDelete
    {
        public virtual string Name { get; set; }

        public virtual bool IsDeleted { get; set; }
    }

A **Person** entity is not actually deleted from database, instead
**IsDeleted** property is set to true when need to delete it. This is
done automatically by ASP.NET Boilerplate when you use
**[IRepository.Delete](/Pages/Documents/Repositories#DocDeleteEntity)**
method (you can manually set IsDeleted to true, but Delete method is
more natural and preffered way).

After implementing ISoftDelete, when you get list of People from
database, deleted people are not retrieved. Here, an example class that
uses a person repository to get all people:

    public class MyService
    {
        private readonly IRepository<Person> _personRepository;

        public MyService(IRepository<Person> personRepository)
        {
            _personRepository = personRepository;
        }

        public List<Person> GetPeople()
        {
            return _personRepository.GetAllList();
        }
    }

GetPeople method only gets Person entities which has IsDeleted = false
(not deleted). All repository methods and also navigation properties
properly works. We could add some other Where conditions, joins.. etc.
It will automatically add IsDeleted = false condition properly to the
generated SQL query.

#### When Enabled?

ISoftDelete filter is always enabled unless you explicitly disable it.

<span class="auto-style1">A side note</span>: If you implement
[IDeletionAudited](/Pages/Documents/Entities#DocSoftDelete) (which
extends ISoftDelete) then deletion time and deleter user id are also
automatically set by ASP.NET Boilerplate.

#### IMustHaveTenant

If you are building multi-tenant applications and store all tenant data
in single database, you definitely do not want a tenant accidently see
other's data. You can implement **IMustHaveTenant** in that case.
Example:

    public class Product : Entity, IMustHaveTenant
    {
        public int TenantId { get; set; }

        public string Name { get; set; }
    }

**IMustHaveTenant** defines **TenantId** to distinguish different tenant
entities. ASP.NET Boilerplate uses
[IAbpSession](/Pages/Documents/Abp-Session) to get current TenantId by
default and automatically filter query for the current tenant.

#### When Enabled?

IMustHaveTenant is enabled by default.

If current user is not logged in to the system or current user is a
**host** user (Host user is an upper level user that can manage tenants
and tenant data), ASP.NET Boilerplate automatically **disables**
IMustHaveTenant filter. Thus, all data of all tenant's can be retrieved
to the application. Notice that this is not about security, you should
always [authorize](/Pages/Documents/Authorization) sensitive data.

#### IMayHaveTenant

If an entity class shared by tenants and the host (that means an entity
object may be owned by a tenant or the host), you can use IMayHaveTenant
filter. **IMayHaveTenant** interface defines **TenantId** but it's
**nullable**.

    public class Role : Entity, IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public string RoleName { get; set; }
    }

A **null** value means this is a **host** entity, a **non-null** value
means this entity owned by a **tenant** which's Id is the TenantId.
ASP.NET Boilerplate uses [IAbpSession](/Pages/Documents/Abp-Session) to
get current TenantId by default. IMayHaveTenant filter is not common as
much as IMustHaveTenant. But you may need it for **common entitiy
types** used by host and tenants.

#### When Enabled?

IMayHaveTenant is always enabled unless you explicitly disable it.

### Disable Filters

You can disable a filter per [unit of
work](/Pages/Documents/Unit-Of-Work) by calling **DisableFilter** method
as shown below:

    var people1 = _personRepository.GetAllList();

    using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
    {
        var people2 = _personRepository.GetAllList();                
    }

    var people3 = _personRepository.GetAllList();

DisableFilter method gets one or more filter names as strings.
AbpDataFilters.SoftDelete is a constant string that contains name of the
standard soft delete filter of ASP.NET Boilerplate.

**people2** will also include deleted people while people1 and people3
will be only non-deleted people. With **using** statement, you can
disable a filter in a **scope**. If you don't use using stamement,
filter will be disabled until end of the current unit of work or you
enable it again explicitly.

You can inject **IUnitOfWorkManager** and use as in the example. Also,
you can use **CurrentUnitOfWork** property as a shortcut if you class
inherits some special base classes (like ApplicationService,
AbpController, AbpApiController...).

#### About using Statement

If a filter is enabled when you call the DisableFilter method with a
using statement, the filter is disabled, then automatically re-enabled
after using statement. But if the filter was already disabled before the
using statement, DisableFilter actually does nothing and the filter
remains disabled even after the using statement.

#### About Multi Tenancy

You can disable tenancy filters to query all tenant data. But remember
that, this works only for single database approach. If you have
seperated databases for each tenants, disabling filter does not help to
query all data of all tenants, since they are in different databases,
can be even in different servers. See [multi tenancy
document](Multi-Tenancy.md) for more information.

#### Disable Filters Globally

If you need, you can disable pre-defined filters globally. For example,
to disable soft delete filter globally, add this code to PreInitialize
method of your module:

    Configuration.UnitOfWork.OverrideFilter(AbpDataFilters.SoftDelete, false);

### Enable Filters

You can enable a filter in a unit of work using **EnableFilter** method,
as similar to (and opposite of) DisableFilter. EnableFilter also returns
disposable to be used in a **using** statement to automatically
re-disable the filter if needed.

### Setting Filter Parameters

A filter can be **parametric**. IMustHaveTenant filter is an example of
these types of filters since current tenant's Id is determined on
runtime. For such filters, we can change filter value if needed.
Example:

    CurrentUnitOfWork.SetFilterParameter("PersonFilter", "personId", 42);

Another example: To set the tenantId value for the IMayHaveTenant
filter:

    CurrentUnitOfWork.SetFilterParameter(AbpDataFilters.MayHaveTenant, AbpDataFilters.Parameters.TenantId, 42);

SetFilterParameter method also returns an IDisposable. So, we can use it
in a **using** statement to automatically **restore the old value**
after using statement.

#### SetTenantId Method

While you can use SetFilterParameter method to change filter value for
MayHaveTenant and MustHaveTenant filters, there is a better way to
change tenant filter: **SetTenantId()**. SetTenantId changes parameter
value for both filters, and also works for single database and database
per tenant approaches. So, it's **always suggested to use SetTenantId**
to change tenancy filter parameter values. See [multi tenancy
document](Multi-Tenancy.md) for more information.

### ORM Integrations

Data filtering for pre-defined filters works for
[NHibernate](NHibernate-Integration.md), [Entity
Framework](EntityFramework-Integration.md) 6.x and [Entity Framework
Core](Entity-Framework-Core.md). Currently, you can only define custom
filters for Entity Framework 6.x.

#### Entity Framework

For [Entity Framework integration](EntityFramework-Integration.md),
automatic data filtering is implemented using
**[EntityFramework.DynamicFilters](https://github.com/jcachat/EntityFramework.DynamicFilters)**
library.

To create a custom filter for Entity Framework and integrate to ASP.NET
Boilerplate, first we should define an interface that will be
implemented by entities which use this filter. Assume that we want to
automatically filter entities by PersonId. Example interface:

    public interface IHasPerson
    {
        int PersonId { get; set; }
    }

Then we can implement this interface for needed entities. Example
entity:

    public class Phone : Entity, IHasPerson
    {
        [ForeignKey("PersonId")]
        public virtual Person Person { get; set; }
        public virtual int PersonId { get; set; }

        public virtual string Number { get; set; }
    }

We use it's rules to define the filter. In our **DbContext** class, we
override **OnModelCreating** and define filter as shown below:

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Filter("PersonFilter", (IHasPerson entity, int personId) => entity.PersonId == personId, 0);
    }

"PersonFilter" is the unique name of the filter here. Second parameter
defines filter interface and personId filter parameter (not needed if
filter is not parametric), last parameter is the default value of the
personId.

As the last thing, we must register this filter to ASP.NET Boilerplate's
unit of work system in PreInitialize method of our
[module](/Pages/Documents/Module-System):

    Configuration.UnitOfWork.RegisterFilter("PersonFilter", false);

First parameter is same unique name we defined before. Second parameter
indicates whether this filter is enabled or disabled by default. After
declaring such a parametric filter, we can use it by supplying it's
value on runtime.

    using (CurrentUnitOfWork.EnableFilter("PersonFilter"))
    {
        using(CurrentUnitOfWork.SetFilterParameter("PersonFilter", "personId", 42))
        {
            var phones = _phoneRepository.GetAllList();
            //...
        }
    }

We could get the personId from some source instead of statically coded.
The example above was for parametric filters. A filter can have zero or
more parameters. If it has no parameter, it's not needed to set the
filter parameter value. Also, if it's enabled by default, no need to
enable it manually (surely, we can disable it).

#### Documentation for EntityFramework.DynamicFilters

For more information on dynamic data filters, see documentation on it's
github page: <https://github.com/jcachat/EntityFramework.DynamicFilters>

We can create custom filters for security, active/passive entities and
so on.

#### Other ORMs

For [Entity Framework Core](Entity-Framework-Core.md) and NHibernate,
data filtering is implemented in the [repository](Repositories.md)
level. That means it only filters when you query over repositories. If
you directly use DbContext (for EF Core) or query via custom SQL, you
should handle filtering yourself.
