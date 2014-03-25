using System;
using System.Text;
using System.Threading;
using Abp.Application.Authorization;
using Abp.Dependency;

namespace Abp.Web.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthenticationScriptManager : IAuthenticationScriptManager, ISingletonDependency
    {
        private readonly IAuthorizationService _authorizationService;

        public AuthenticationScriptManager(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public string GetAuthenticationScript()
        {
            var allPermission = _authorizationService.GetAllPermissionNames();
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

        private void AppendPermissionList(StringBuilder script, string name, string[] permissions)
        {
            script.AppendLine("    abp.auth." + name + " = {");

            for (var i = 0; i < permissions.Length; i++)
            {
                var permission = permissions[i];
                if (i < permissions.Length - 1)
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