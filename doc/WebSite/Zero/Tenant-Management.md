#### About Multi Tenancy

It's strongly suggested to read [multi tenancy
documentation](/Pages/Documents/Multi-Tenancy) before this document.

### Enabling Multi Tenancy

ASP.NET Boilerplate and module-zero can run **multi-tenant** or
**single-tenant** modes. Multi-tenancy is disabled by default. We can
enable it in PreInitialize method of our
[module](/Pages/Documents/Module-System) as shown below:

    [DependsOn(typeof(AbpZeroCoreModule))]
    public class MyCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.MultiTenancy.IsEnabled = true;    
        }

        ...
    }

Note that: Even our application is not multi-tenant, we must define a
default tenant (see Default Tenant section of this document). 

When we create a project [template](/Templates) based on ASP.NET
Boilerplate and module-zero, we have a **Tenant** entity and
**TenantManager** domain service.

### Tenant Entity

Tenant entity represents a Tenant of the application.

    public class Tenant : AbpTenant<Tenant, User>
    {

    }

It's derived from generic **AbpTenant** class. Tenant entities are
stored in **AbpTenants** table in the database. You can add your custom
properties to Tenant class.

AbpTenant class defines some base properties, most importants are:

-   **TenancyName**: This is **unique** name of a tenant in the
    application. It should not be changed normally. It can be used to
    allocate subdomains to tenants like '**mytenant**.mydomain.com'.
    Tenant.**TenancyNameRegex** constant defines the naming rule.
-   **Name**: An arbitrary, human-readable, long name of the tenant.
-   **IsActive**: True, if this tenant can use the application. If it's
    false, no user of this tenant can login to to system.

AbpTenant class is inherited from **FullAuditedEntity**. That means it
has creation, modification and deletion **audit properties**. It's also
**[Soft-Delete](/Pages/Documents/Data-Filters#DocSoftDelete)** . So,
when we delete a tenant, it's not deleted from database, just marked as
deleted.

Finally, **Id** of AbpTenant is defined as **int**.

### Tenant Manager

Tenant Manager is a service to perform **domain logic** for tenants:

    public class TenantManager : AbpTenantManager<Tenant, Role, User>
    {
        public TenantManager(IRepository<Tenant> tenantRepository)
            : base(tenantRepository)
        {

        }
    }

TenantManager is also used to manage tenant
[features](/Pages/Documents/Feature-Management). You can add your own
methods here. Also you can override any method of AbpTenantManager base
class for your own needs.

### Default Tenant

ASP.NET Boilerplate and module-zero assumes that there is a pre-defined
tenant which's TenancyName is '**Default**' and Id is **1**. In a
single-tenant application, this is used as as the single tenant. In a
multi-tenant application, you can delete it or make it passive.
