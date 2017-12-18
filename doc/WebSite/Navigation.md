Every web application has some menu to navigate between pages/screens.
ASP.NET Boilerplate provides a common ifrastructure to create and show
menu to users.

### Creating Menus

An application may be consists of different
[modules](/Pages/Documents/Module-System) and each module can have it's
own menu items. To define menu items, we need to create a class derived
from **NavigationProvider**.

Assume that we have a main menu as shown below:

-   Tasks
-   Reports
-   Administration
    -   User management
    -   Role management

Here, Administration menu item has two **sub menu items**. Example
navigation provider class to create such a menu can be as below:

    public class SimpleTaskSystemNavigationProvider : NavigationProvider
    {
        public override void SetNavigation(INavigationProviderContext context)
        {
            context.Manager.MainMenu
                .AddItem(
                    new MenuItemDefinition(
                        "Tasks",
                        new LocalizableString("Tasks", "SimpleTaskSystem"),
                        url: "/Tasks",
                        icon: "fa fa-tasks"
                        )
                ).AddItem(
                    new MenuItemDefinition(
                        "Reports",
                        new LocalizableString("Reports", "SimpleTaskSystem"),
                        url: "/Reports",
                        icon: "fa fa-bar-chart"
                        )
                ).AddItem(
                    new MenuItemDefinition(
                        "Administration",
                        new LocalizableString("Administration", "SimpleTaskSystem"),
                        icon: "fa fa-cogs"
                        ).AddItem(
                            new MenuItemDefinition(
                                "UserManagement",
                                new LocalizableString("UserManagement", "SimpleTaskSystem"),
                                url: "/Administration/Users",
                                icon: "fa fa-users",
                                requiredPermissionName: "SimpleTaskSystem.Permissions.UserManagement"
                                )
                        ).AddItem(
                            new MenuItemDefinition(
                                "RoleManagement",
                                new LocalizableString("RoleManagement", "SimpleTaskSystem"),
                                url: "/Administration/Roles",
                                icon: "fa fa-star",
                                requiredPermissionName: "SimpleTaskSystem.Permissions.RoleManagement"
                                )
                        )
                );
        }
    }

A MenuItemDefinition can basically have a unique **name**, a localizable
**display name**, a **url** and an **icon**. Also,

-   A menu item may require a permission to show this menu to a
    particular user (See [authorization](/Pages/Documents/Authorization)
    document). **requiredPermissionName** property can be used in this
    case.
-   A menu item can be depend on a
    [feature](/Pages/Documents/Feature-Management).
    **featureDependency** property can be used in this case.
-   A menu item can define a **customData** and **order**.

**INavigationProviderContext** has methods to get existing menu items,
add menus and menu items. Thus, different modules can add it's own items
to the menu.

There may be one or more menus in an application.
**context.Manager.MainMenu** references the default, main menu. We can
create and add more menus using **context.Manager.Menus** property.

#### Registering Navigation Provider

After creating the navigation provider, we should register it to ASP.NET
Boilerplate configuration on **PreInitialize** event of our
[module](/Pages/Documents/Module-System):

    Configuration.Navigation.Providers.Add<SimpleTaskSystemNavigationProvider>(); 

### Showing Menu

**IUserNavigationManager** can be
[injected](/Pages/Documents/Dependency-Injection) and used to get menu
items and show to the user. Thus, we can create menu in server side.

ASP.NET Boilerplate automatically generates a **javascript API** to get
menu and items in client side. Methods and objects under **abp.nav**
namespace can be used for this purpose. For instance,
**abp.nav.menus.MainMenu** can be used to get main menu of the
application. Thus, we can create menu in client side.

ASP.NET Boilerplate [templates](/Templates) uses this system to create
and show menu to the user. Try to create a template and see source codes
for more.
