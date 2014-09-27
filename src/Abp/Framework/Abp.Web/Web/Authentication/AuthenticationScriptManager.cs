using System.Collections.Generic;
using System.Linq;
using System.Text;
using Abp.Authorization;
using Abp.Authorization.Permissions;
using Abp.Dependency;

namespace Abp.Web.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthenticationScriptManager : IAuthenticationScriptManager, ISingletonDependency
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IPermissionManager _permissionManager;

        public AuthenticationScriptManager(IAuthorizationService authorizationService, IPermissionManager permissionManager)
        {
            _authorizationService = authorizationService;
            _permissionManager = permissionManager;
        }

        public string GetAuthenticationScript()
        {
            var allPermission = _permissionManager.GetAllPermissions().Select(p => p.Name).ToList();
            var grantedPermissions = _authorizationService.GetGrantedPermissionNames();
            
            var script = new StringBuilder();

            script.AppendLine("(function(){");
            
            script.AppendLine();
            
            script.AppendLine("    abp.auth = abp.auth || {};");
            
            script.AppendLine();

            AppendPermissionList(script, "allPermissions", allPermission);
            
            script.AppendLine();
            
            AppendPermissionList(script, "grantedPermissions", grantedPermissions);

            script.AppendLine();
            script.Append("})();");

            return script.ToString();
        }

        private void AppendPermissionList(StringBuilder script, string name, IReadOnlyList<string> permissions)
        {
            script.AppendLine("    abp.auth." + name + " = {");

            for (var i = 0; i < permissions.Count; i++)
            {
                var permission = permissions[i];
                if (i < permissions.Count - 1)
                {
                    script.AppendLine("        '" + permission + "': true,");
                }
                else
                {
                    script.AppendLine("        '" + permission + "': true");
                }
            }

            script.AppendLine("    };");
        }
    }
}