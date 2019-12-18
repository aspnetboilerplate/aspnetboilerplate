using System.Text;
using System.Threading.Tasks;
using Abp.Application.Navigation;
using Abp.Dependency;
using Abp.Json;
using Abp.Runtime.Session;
using Abp.Web.Http;

namespace Abp.Web.Navigation
{
    internal class NavigationScriptManager : INavigationScriptManager, ITransientDependency
    {
        public IAbpSession AbpSession { get; set; }

        private readonly IUserNavigationManager _userNavigationManager;

        public NavigationScriptManager(IUserNavigationManager userNavigationManager)
        {
            _userNavigationManager = userNavigationManager;
            AbpSession = NullAbpSession.Instance;
        }

        public async Task<string> GetScriptAsync()
        {
            var userMenus = await _userNavigationManager.GetMenusAsync(AbpSession.ToUserIdentifier());

            var script = new StringBuilder();
            script.AppendLine("(function() {");

            script.AppendLine("    abp.nav = {};");
            script.AppendLine("    abp.nav.menus = {");

            for (int i = 0; i < userMenus.Count; i++)
            {
                AppendMenu(script, userMenus[i]);
                if (userMenus.Count - 1 > i)
                {
                    script.Append(" , ");
                }
            }

            script.AppendLine("    };");

            script.AppendLine("})();");

            return script.ToString();
        }

        private static void AppendMenu(StringBuilder script, UserMenu menu)
        {
            script.AppendLine("        '" + HttpEncode.JavaScriptStringEncode(menu.Name) + "': {");

            script.AppendLine("            name: '" + HttpEncode.JavaScriptStringEncode(menu.Name) + "',");

            if (menu.DisplayName != null)
            {
                script.AppendLine("            displayName: '" + HttpEncode.JavaScriptStringEncode(menu.DisplayName) + "',");
            }

            if (menu.CustomData != null)
            {
                script.AppendLine("            customData: " + menu.CustomData.ToJsonString(true) + ",");
            }

            script.Append("            items: ");

            if (menu.Items.Count <= 0)
            {
                script.AppendLine("[]");
            }
            else
            {
                script.Append("[");
                for (int i = 0; i < menu.Items.Count; i++)
                {
                    AppendMenuItem(16, script, menu.Items[i]);
                    if (menu.Items.Count - 1 > i)
                    {
                        script.Append(" , ");
                    }
                }
                script.AppendLine("]");
            }

            script.AppendLine("            }");
        }

        private static void AppendMenuItem(int indentLength, StringBuilder sb, UserMenuItem menuItem)
        {
            sb.AppendLine("{");

            sb.AppendLine(new string(' ', indentLength + 4) + "name: '" + HttpEncode.JavaScriptStringEncode(menuItem.Name) + "',");
            sb.AppendLine(new string(' ', indentLength + 4) + "order: " + menuItem.Order + ",");

            if (!string.IsNullOrEmpty(menuItem.Icon))
            {
                sb.AppendLine(new string(' ', indentLength + 4) + "icon: '" + HttpEncode.JavaScriptStringEncode(menuItem.Icon) + "',");
            }

            if (!string.IsNullOrEmpty(menuItem.Url))
            {
                sb.AppendLine(new string(' ', indentLength + 4) + "url: '" + HttpEncode.JavaScriptStringEncode(menuItem.Url) + "',");
            }

            if (menuItem.DisplayName != null)
            {
                sb.AppendLine(new string(' ', indentLength + 4) + "displayName: '" + HttpEncode.JavaScriptStringEncode(menuItem.DisplayName) + "',");
            }

            if (menuItem.CustomData != null)
            {
                sb.AppendLine(new string(' ', indentLength + 4) + "customData: " + menuItem.CustomData.ToJsonString(true) + ",");
            }

            if (menuItem.Target != null)
            {
                sb.AppendLine(new string(' ', indentLength + 4) + "target: '" + HttpEncode.JavaScriptStringEncode(menuItem.Target) + "',");
            }

            sb.AppendLine(new string(' ', indentLength + 4) + "isEnabled: " + menuItem.IsEnabled.ToString().ToLowerInvariant() + ",");
            sb.AppendLine(new string(' ', indentLength + 4) + "isVisible: " + menuItem.IsVisible.ToString().ToLowerInvariant() + ",");

            sb.Append(new string(' ', indentLength + 4) + "items: [");

            for (int i = 0; i < menuItem.Items.Count; i++)
            {
                AppendMenuItem(24, sb, menuItem.Items[i]);
                if (menuItem.Items.Count - 1 > i)
                {
                    sb.Append(" , ");
                }
            }

            sb.AppendLine("]");

            sb.Append(new string(' ', indentLength) + "}");
        }
    }
}