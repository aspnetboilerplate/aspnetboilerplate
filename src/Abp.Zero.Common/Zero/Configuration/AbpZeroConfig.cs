namespace Abp.Zero.Configuration
{
    internal class AbpZeroConfig : IAbpZeroConfig
    {
        public IRoleManagementConfig RoleManagement
        {
            get { return _roleManagementConfig; }
        }
        private readonly IRoleManagementConfig _roleManagementConfig;

        public IUserManagementConfig UserManagement
        {
            get { return _userManagementConfig; }
        }
        private readonly IUserManagementConfig _userManagementConfig;

        public ILanguageManagementConfig LanguageManagement
        {
            get { return _languageManagement; }
        }
        private readonly ILanguageManagementConfig _languageManagement;

        public IAbpZeroEntityTypes EntityTypes
        {
            get { return _entityTypes; }
        }
        private readonly IAbpZeroEntityTypes _entityTypes;


        public AbpZeroConfig(
            IRoleManagementConfig roleManagementConfig,
            IUserManagementConfig userManagementConfig,
            ILanguageManagementConfig languageManagement,
            IAbpZeroEntityTypes entityTypes)
        {
            _entityTypes = entityTypes;
            _roleManagementConfig = roleManagementConfig;
            _userManagementConfig = userManagementConfig;
            _languageManagement = languageManagement;
        }
    }
}