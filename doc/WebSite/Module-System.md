### Introduction

ASP.NET Boilerplate provides the infrastructure to build modules and
compose them to create an application. A module can depend on another
module. Generally, an assembly is considered a module. If you create
an application with more than one assembly, it's recommended that you create a
module definition for each one.

The module system is currently focused on the server-side rather than client-side.

### Module Definition

A module is defined with a class that is derived from **AbpModule** that is in the [ABP package](https://www.nuget.org/packages/Abp). Say
that we're developing a Blog module that can be used in different
applications. The simplest module definition can be as shown below:

    public class MyBlogApplicationModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }

The Module definition class is responsible for registering its classes via
[dependency injection](Dependency-Injection.md), if needed (it can be done
conventionally as shown above). It can also configure the application
and other modules, add new features to the application, and so on...

### Lifecycle Methods

ASP.NET Boilerplate calls some specific methods of modules on
application startup and shutdown. You can override these methods to
perform some specific tasks.

ASP.NET Boilerplate calls these methods **ordered by dependencies**. If
module A depends on module B, module B is initialized before module A.

The exact order of startup methods: PreInitialize-B, PreInitialize-A,
Initialize-B, Initialize-A, PostInitialize-B and PostInitialize-A. This
is true for all dependency graphs. The **shutdown** method is also similar,
but in **reverse order**.

#### PreInitialize

This method is called first, when the application starts. It's the go-to method
to [configure](Startup-Configuration.md) the framework and other
modules before they initialize.

You can also write some specific code here to run before the dependency
injection registrations. For example, if you create a [conventional
registration](Dependency-Injection.md) class, you should register it
here using the IocManager.AddConventionalRegisterer method.

#### Initialize

This is the place where [dependency
injection](/Pages/Documents/Dependency-Injection) registration should be
done. It's generally done using the IocManager.RegisterAssemblyByConvention
method. If you want to define custom dependency registration, see the
[dependency injection documentation](Dependency-Injection.md).

#### PostInitialize

This method is called last in the startup process. It's safe to resolve a
dependency here.

#### Shutdown

This method is called when the application shuts down.

### Module Dependencies

A module can be dependent on another. You need to **explicitly**
declare the dependencies using the **DependsOn** attribute, like below:

    [DependsOn(typeof(MyBlogCoreModule))]
    public class MyBlogApplicationModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }

Here, we declare to ASP.NET Boilerplate that MyBlogApplicationModule
depends on the MyBlogCoreModule and the MyBlogCoreModule should be
initialized before the MyBlogApplicationModule.

ABP can resolve dependencies recursively beginning from the **startup
module** and initialize them accordingly. The startup module initializes as
the last module.

### PlugIn Modules

While modules are investigated beginning from the startup module and go
through the dependencies, ABP can also load modules **dynamically**.
The **AbpBootstrapper** class defines the **PlugInSources** property which can
be used to add sources to dynamically loaded [plugin modules](Plugin.md). A plugin
source can be any class implementing the **IPlugInSource** interface.
The **PlugInFolderSource** class implements it to get the plugin modules from
assemblies located in a folder.

#### ASP.NET Core

The ABP ASP.NET Core module defines options in the **AddAbp** extension method
to add plugin sources in the **Startup** class:

    services.AddAbp<MyStartupModule>(options =>
    {
        options.PlugInSources.Add(new FolderPlugInSource(@"C:\MyPlugIns"));
    });

We could use the **AddFolder** extension method for a simpler syntax:

    services.AddAbp<MyStartupModule>(options =>
    {
        options.PlugInSources.AddFolder(@"C:\MyPlugIns");
    });

See the [ASP.NET Core document](AspNet-Core.md) for more info on the Startup class.

#### ASP.NET MVC, Web API

For classic ASP.NET MVC applications, we can add plugin folders by
overriding the **Application\_Start** in the **global.asax** as shown below:

    public class MvcApplication : AbpWebApplication<MyStartupModule>
    {
        protected override void Application_Start(object sender, EventArgs e)
        {
            AbpBootstrapper.PlugInSources.AddFolder(@"C:\MyPlugIns");
            //...
            base.Application_Start(sender, e);
        }
    }

##### Controllers in PlugIns

If your modules include MVC or Web API Controllers,
ASP.NET cannot investigate your controllers. To overcome this issue,
you can change the global.asax file like below:

    using System.Web;
    using Abp.PlugIns;
    using Abp.Web;
    using MyDemoApp.Web;

    [assembly: PreApplicationStartMethod(typeof(PreStarter), "Start")]

    namespace MyDemoApp.Web
    {
        public class MvcApplication : AbpWebApplication<MyStartupModule>
        {
        }

        public static class PreStarter
        {
            public static void Start()
            {
                //...
                MvcApplication.AbpBootstrapper.PlugInSources.AddFolder(@"C:\MyPlugIns\");
                MvcApplication.AbpBootstrapper.PlugInSources.AddToBuildManager();
            }
        }
    }

### Additional Assemblies

The default implementations for IAssemblyFinder and ITypeFinder (which is
used by ABP to investigate specific classes in the application) only
finds module assemblies and types in those assemblies. We can override the
**GetAdditionalAssemblies** method in our module to include additional
assemblies.

### Custom Module Methods

Your modules can also have custom methods that can be used by other
modules that depend on this module. Assume that MyModule2 depends on
MyModule1 and wants to call a method of MyModule1 in the PreInitialize method.

    public class MyModule1 : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        public void MyModuleMethod1()
        {
            //this is a custom method of this module
        }
    }

    [DependsOn(typeof(MyModule1))]
    public class MyModule2 : AbpModule
    {
        private readonly MyModule1 _myModule1;

        public MyModule2(MyModule1 myModule1)
        {
            _myModule1 = myModule1;
        }

        public override void PreInitialize()
        {
            _myModule1.MyModuleMethod1(); //Call MyModule1's method
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }

Here we constructor-injected MyModule1 to MyModule2, so MyModule2 can
call MyModule1's custom method. This is only possible if Module2 depends
on Module1.

### Module Configuration

While custom module methods can be used to configure modules, we suggest
you use the [startup configuration](Startup-Configuration.md) system to
define and set the configuration for modules.

### Module Lifetime

Module classes are automatically registered as a **singleton**.
