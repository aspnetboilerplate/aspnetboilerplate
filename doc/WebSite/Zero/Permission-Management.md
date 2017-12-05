#### About Authorization

It's strongly suggested to read [authorization
documentation](/Pages/Documents/Authorization) before this document.

### Introduction

Module-zero implements **IPermissionChecker** interface of ASP.NET
Boilerplate's authorization system. To define and check permissions, you
can see [authorization document](/Pages/Documents/Authorization). In
this document, we will see how to grant permissions for roles and users.

### Role Permissions

If we **grant** a role for a permission, all users have this role are
authorized for the permission (unless explicitly prohibited for a
specific user).

We use **RoleManager** to change permissions of a Role. For example,
**SetGrantedPermissionsAsync** can be used to change all permissions of
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

In this example, we get a **RoleId** and list of granted permission
names (input.GrantedPermissionNames is a List&lt;string&gt;) as input.
We use **IPermissionManager** to find all **Permission** objects by
name. Then we call **SetGrantedPermissionsAsync** method to update
permissions of the role.

There are also other methods like GrantPermissionAsync and
ProhibitPermissionAsync to control permissions one by one.

### User Permissions

While role-based permission management can be enough for most
applications, we may need to control permissions per user. When we
define a permission setting for a user, it overrides permission setting
comes from roles of the user.

As an example; Assume that we have an application service to prohibit a
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

UserManager has many methods to control permissions for users. In this
example, we're getting a UserId and PermissionName and using
UserManager's **ProhibitPermissionAsync** method to prohibit a
permission for a user.

When we **prohibit** a permission for a user, he/she **can not** be
authorized for this permission even his/her roles are **granted** for
the permission. We can say same principle for granting. When we
**grant** a permission specifically for a user, this user **is granted**
for the permission even roles of the user are not granted for the
permission. We can use **ResetAllPermissionsAsync** for a user to delete
all user-specific permission settings for the user.
