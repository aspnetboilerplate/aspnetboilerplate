Every web application has some kind of menu to navigate between pages/screens.
ASP.NET Boilerplate provides a common infrastructure to create and show
a menu to users.

### Creating Menus

An application may consist of different
[modules](/Pages/Documents/Module-System) and each module may have it's
own menu items. To define menu items, we need to create a class derived
from the **NavigationProvider**.

Imagine that we have a main menu like the one shown below:

-   Tasks
-   Reports
-   Administration
    -   User management
    -   Role management

Here, the Administration menu item has two **sub menu items**. Here's a
navigation provider class to create such a menu:

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
**display name**, an **url** and an **icon**. Also:

-   A menu item may require a permission to show this menu to a
    particular user (See the [authorization](/Pages/Documents/Authorization)
    document). The **requiredPermissionName** property can be used in this
    case.
-   A menu item can be dependent on a
    [feature](/Pages/Documents/Feature-Management).
    The **featureDependency** property can be used in this case.
-   A menu item can define **customData** and the **order** in which it appears.

The **INavigationProviderContext** has methods to get existing menu items,
add menus, and edit menu items. This way, different modules can add their own items
to the menu.

There may be one or more menus in an application.
The **context.Manager.MainMenu** references the default main menu. We can
create and add more menus using the **context.Manager.Menus** property.

#### Registering Navigation Provider

After creating the navigation provider, we need to register it to ASP.NET
Boilerplate's configuration on the **PreInitialize** method of our
[module](/Pages/Documents/Module-System):

    Configuration.Navigation.Providers.Add<SimpleTaskSystemNavigationProvider>(); 

### Showing Menu

The **IUserNavigationManager** can be
[injected](/Pages/Documents/Dependency-Injection) and used to get menu
items and show them to the user. This way, we can create a menu on the server-side.

ASP.NET Boilerplate automatically generates a **JavaScript API** to get the
menu and items on the client-side. Methods and objects under the **abp.nav**
namespace can be used for this purpose. For instance,
**abp.nav.menus.MainMenu** can be used to get the main menu of the
application. This way, we can create a menu on the client-side.

The ASP.NET Boilerplate [templates](/Templates) use this system to create
and show a menu to the user. Create a template and see the source code
for more info.
