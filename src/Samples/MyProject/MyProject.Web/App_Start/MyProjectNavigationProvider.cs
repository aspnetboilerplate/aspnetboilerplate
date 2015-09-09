using Abp.Application.Navigation;
using Abp.Localization;

namespace MyProject.Web
{
    /// <summary>
    /// This class defines menus for the application.
    /// It uses ABP's menu system.
    /// When you add menu items here, they are automatically appear in angular application.
    /// See Views/Layout/_TopMenu.cshtml file to know how to render menu.
    /// </summary>
    public class MyProjectNavigationProvider : NavigationProvider
    {
        public override void SetNavigation(INavigationProviderContext context)
        {
            context.Manager.MainMenu
                .AddItem(
                    new MenuItemDefinition(
                        "Home",
                        new LocalizableString("HomePage", MyProjectConsts.LocalizationSourceName),
                        url: "/",
                        icon: "fa fa-home"
                        )
                ).AddItem(
                    new MenuItemDefinition(
                        "About",
                        new LocalizableString("About", MyProjectConsts.LocalizationSourceName),
                        url: "/About",
                        icon: "fa fa-info"
                        )
                ).AddItem(
                   new MenuItemDefinition(
                       "TaskList",
                       new LocalizableString("TaskList", MyProjectConsts.LocalizationSourceName),
                       url: "/Home/TaskList",
                       icon: "fa fa-tasks"
                       )
               ).AddItem(
                   new MenuItemDefinition(
                       "NewTask",
                       new LocalizableString("NewTask", MyProjectConsts.LocalizationSourceName),
                       url: "/Home/NewTask",
                       icon: "fa fa-asterisk"
                       )
               );
            
        }
    }
}
