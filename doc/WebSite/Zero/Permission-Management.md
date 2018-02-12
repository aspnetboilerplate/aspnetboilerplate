#### About Authorization

We strongly recommend that you read the [authorization
documentation](/Pages/Documents/Authorization) before this one.

### Introduction

Module Zero implements the **IPermissionChecker** interface of ASP.NET
Boilerplate's authorization system. To define and check permissions,
see the [authorization document](/Pages/Documents/Authorization). In
this document, we will show you how to grant permissions for roles and users.

### Role Permissions

If we **grant** a permission to a role, all the users that have this role are
authorized for the permission (unless explicitly prohibited for a
specific user).

We use the **RoleManager** to change the permissions of a Role. For example,
**SetGrantedPermissionsAsync** can be used to change all the permissions of
a role in one method call:

    public class RoleAppService : IRoleAppService
    {
        private readonly RoleManager _roleManager;
        private readonly IPermissionManager _permissionManager;

        public RoleAppService(RoleManager roleManager, IPermissionManager permissionManager)
        {
            _roleManager = roleManager;
            _permissionManager = permissionManager;
        }

        public async Task UpdateRolePermissions(UpdateRolePermissionsInput input)
        {
            var role = await _roleManager.GetRoleByIdAsync(input.RoleId);
            var grantedPermissions = _permissionManager
                .GetAllPermissions()
                .Where(p => input.GrantedPermissionNames.Contains(p.Name))
                .ToList();

            await _roleManager.SetGrantedPermissionsAsync(role, grantedPermissions);
        }
    }

In this example, we get a **RoleId** and a list of granted permission
names (input.GrantedPermissionNames is a List&lt;string&gt;) as an input.
We then use the **IPermissionManager** to find all the **Permission** objects by
name. After that, we call the **SetGrantedPermissionsAsync** method to update
the permissions of the role.

There are also other methods like GrantPermissionAsync and
ProhibitPermissionAsync to control the permissions one-by-one.

### User Permissions

While the role-based permission management can be enough for most
applications, we may need to control the permissions per user. When we
define a permission setting for a user, it overrides the permission setting
defined for the roles of the user.

As an example, imagine that we have an application service method for prohibiting a
permission for a user:

    public class UserAppService : IUserAppService
    {
        private readonly UserManager _userManager;
        private readonly IPermissionManager _permissionManager;

        public UserAppService(UserManager userManager, IPermissionManager permissionManager)
        {
            _userManager = userManager;
            _permissionManager = permissionManager;
        }

        public async Task ProhibitPermission(ProhibitPermissionInput input)
        {
            var user = await _userManager.GetUserByIdAsync(input.UserId);
            var permission = _permissionManager.GetPermission(input.PermissionName);

            await _userManager.ProhibitPermissionAsync(user, permission);
        }
    }

The UserManager has many methods to control the permissions of users. In this
example, we're getting a UserId and PermissionName and using the
UserManager's **ProhibitPermissionAsync** method to prohibit a
permission for a user.

When we **prohibit** a permission for a user, he/she **cannot** be
authorized for this permission even his/her roles are **granted** for
the permission. We can use the same principle for granting permissions. When we
**grant** a permission specifically for a user, this user **is granted**
the permission even if the roles of the user are not granted the
permission. We can use the **ResetAllPermissionsAsync** method to delete
all user-specific permission settings for a user.
