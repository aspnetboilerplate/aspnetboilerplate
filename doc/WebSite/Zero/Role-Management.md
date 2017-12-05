### Role Entity

Role entity represents a **role for the application**. It should be
derived from **AbpRole** class as shown below:

    public class Role : AbpRole<Tenant, User>
    {
        //add your own role properties here
    }

This class is created when you
[install](/Pages/Documents/Zero/Installation) module-zero. Roles are
stored in **AbpRoles** table in the database. You can add your custom
properties to Role class (and create database migrations for the
changes).

AbpRole defines some properties. Most importants are:

-   **Name**: Unique name of the role in the tenant.
-   **DisplayName**: Shown name of the role.
-   **IsDefault**: Is this role assigned to new users by default?
-   **IsStatic**: Is this role static (pre-build and can not be
    deleted).

Roles are used to **group permissions**. When a user has a role, then
he/she will have all permissions of that role. A user can have
**multiple** roles. Permissions of this user will be a merge of all
permissions of all assigned roles.

#### Dynamic vs Static Roles

In module-zero, roles can be dynamic or static:

-   **Static role**: A static role has a known **name** (like 'admin')
    and can not change this name (we can change **display name**). It
    exists on the system startup and can not be deleted. Thus, we can
    write codes based on a static role name.
-   **Dynamic (non static) role**: We can create a dynamic role after
    deployment. Then we can grant permissions for that role, we can
    assign the role to some users and we can delete it. We can not know
    names of dynamic roles in development time.

Use **IsStatic** property to set it for a role. Also, we should
**register** static roles on
[PreInitialize](/Pages/Documents/Module-System) of our module. Assume
that we have an "Admin" static role for tenants:

    Configuration.Modules.Zero().RoleManagement.StaticRoles.Add(new StaticRoleDefinition("Admin", MultiTenancySides.Tenant));

Thus, module-zero will be aware of static roles.

#### Default Roles

One or more roles can be set as **default**. Default roles are assigned
to new added/registered users as default. This is not a development time
property and can be set or changed after deployment. Use **IsDefault**
property to set it.

### Role Manager

**RoleManager** is a service to perform **domain logic** for roles:

    public class RoleManager : AbpRoleManager<Tenant, Role, User>
    {
        //...
    }

You can [inject](/Pages/Documents/Dependency-Injection) and use
RoleManager to create, delete, update roles, grant permissions for roles
and much more. You can add your own methods here. Also, you can
**override** any method of **AbpRoleManager** base class for your own
needs.

Like UserManager, Some methods of RoleManager also return IdentityResult
as a result instead of throwing exceptions for some cases. See [user
management](/Pages/Documents/Zero/User-Management) document for more
information.

### Multi Tenancy

Similar to user management, role management also works for single tenant
in one time in a multi-tenant application. See [user
management](/Pages/Documents/Zero/User-Management#multi-tenancy)
document for more information.
