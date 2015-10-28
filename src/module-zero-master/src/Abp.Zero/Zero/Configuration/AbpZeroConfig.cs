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
            get
            {
                return _userManagementConfig;
            }
        }
        private readonly IUserManagementConfig _userManagementConfig;

        public AbpZeroConfig(IRoleManagementConfig roleManagementConfig, IUserManagementConfig userManagementConfig)
        {
            _roleManagementConfig = roleManagementConfig;
            _userManagementConfig = userManagementConfig;
        }
    }
}