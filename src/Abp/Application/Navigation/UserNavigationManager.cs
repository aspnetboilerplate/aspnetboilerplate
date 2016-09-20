using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Features;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Localization;
using Abp.MultiTenancy;
using Abp.Runtime.Session;

namespace Abp.Application.Navigation
{
    internal class UserNavigationManager : IUserNavigationManager, ITransientDependency
    {
        public IPermissionChecker PermissionChecker { get; set; }

        public IAbpSession AbpSession { get; set; }

        private readonly INavigationManager _navigationManager;
        private readonly ILocalizationContext _localizationContext;
        private readonly IIocResolver _iocResolver;

        public UserNavigationManager(
            INavigationManager navigationManager,
            ILocalizationContext localizationContext,
            IIocResolver iocResolver)
        {
            _navigationManager = navigationManager;
            _localizationContext = localizationContext;
            _iocResolver = iocResolver;
            PermissionChecker = NullPermissionChecker.Instance;
            AbpSession = NullAbpSession.Instance;
        }

        [Obsolete("Use GetMenuAsync(UserIdentifier) instead.")]
        public Task<UserMenu> GetMenuAsync(string menuName, long? userId, int? tenantId = null)
        {
            return GetMenuAsync(menuName, GetNormalizedUserIdentifier(tenantId, userId));
        }

        public async Task<UserMenu> GetMenuAsync(string menuName, UserIdentifier user)
        {
            var menuDefinition = _navigationManager.Menus.GetOrDefault(menuName);
            if (menuDefinition == null)
            {
                throw new AbpException("There is no menu with given name: " + menuName);
            }

            var userMenu = new UserMenu(menuDefinition, _localizationContext);
            await FillUserMenuItems(user, menuDefinition.Items, userMenu.Items);
            return userMenu;
        }

        [Obsolete("Use GetMenusAsync(UserIdentifier) instead.")]
        public Task<IReadOnlyList<UserMenu>> GetMenusAsync(long? userId, int? tenantId = null)
        {
            return GetMenusAsync(GetNormalizedUserIdentifier(tenantId, userId));
        }

        public async Task<IReadOnlyList<UserMenu>> GetMenusAsync(UserIdentifier user)
        {
            var userMenus = new List<UserMenu>();

            foreach (var menu in _navigationManager.Menus.Values)
            {
                userMenus.Add(await GetMenuAsync(menu.Name, user));
            }

            return userMenus;
        }

        private async Task<int> FillUserMenuItems(UserIdentifier user, IList<MenuItemDefinition> menuItemDefinitions, IList<UserMenuItem> userMenuItems)
        {
            //TODO: Can be optimized by re-using FeatureDependencyContext.

            var addedMenuItemCount = 0;

            using (var featureDependencyContext = _iocResolver.ResolveAsDisposable<FeatureDependencyContext>())
            {
                featureDependencyContext.Object.TenantId = user == null ? null : user.TenantId;

                foreach (var menuItemDefinition in menuItemDefinitions)
                {
                    if (menuItemDefinition.RequiresAuthentication && user == null)
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(menuItemDefinition.RequiredPermissionName) && (user == null || !(await PermissionChecker.IsGrantedAsync(user, menuItemDefinition.RequiredPermissionName))))
                    {
                        continue;
                    }

                    if (menuItemDefinition.FeatureDependency != null &&
                        (AbpSession.MultiTenancySide == MultiTenancySides.Tenant || (user != null && user.TenantId != null)) &&
                        !(await menuItemDefinition.FeatureDependency.IsSatisfiedAsync(featureDependencyContext.Object)))
                    {
                        continue;
                    }

                    var userMenuItem = new UserMenuItem(menuItemDefinition, _localizationContext);
                    if (menuItemDefinition.IsLeaf || (await FillUserMenuItems(user, menuItemDefinition.Items, userMenuItem.Items)) > 0)
                    {
                        userMenuItems.Add(userMenuItem);
                        ++addedMenuItemCount;
                    }
                }
            }

            return addedMenuItemCount;
        }

        private UserIdentifier GetNormalizedUserIdentifier(int? tenantId, long? userId)
        {
            if (userId == null)
            {
                //No identifier
                return null;
            }

            if (tenantId != null)
            {
                //known tenant id
                return new UserIdentifier(tenantId, userId.Value);
            }

            if (userId == AbpSession.UserId)
            {
                //normalized tenant id
                return new UserIdentifier(AbpSession.TenantId, userId.Value);
            }

            //assumed as host user
            return new UserIdentifier(null, userId.Value);
        }
    }
}
