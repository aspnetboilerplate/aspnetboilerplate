ASP.NET Boilerplate provides the infrastructure and a model to configure
its [modules](/Pages/Documents/Module-System) on startup.

### Configuring ASP.NET Boilerplate

Configuring ASP.NET Boilerplate is made on the **PreInitialize** method of
your module. Example configuration:

    public class SimpleTaskSystemModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Add languages for your application
            Configuration.Localization.Languages.Add(new LanguageInfo("en", "English", "famfamfam-flag-england", true));
            Configuration.Localization.Languages.Add(new LanguageInfo("tr", "Türkçe", "famfamfam-flag-tr"));

            //Add a localization source
            Configuration.Localization.Sources.Add(
                new XmlLocalizationSource(
                    "SimpleTaskSystem",
                    HttpContext.Current.Server.MapPath("~/Localization/SimpleTaskSystem")
                    )
                );

            //Configure navigation/menu
            Configuration.Navigation.Providers.Add<SimpleTaskSystemNavigationProvider>();        
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }

ASP.NET Boilerplate is designed with **[modularity](Module-System.md)** in
mind. Different modules can configure ASP.NET Boilerplate. For example,
different modules can add navigation providers to add their own menu
items to the main menu. (See the
[localization](/Pages/Documents/Localization) and
[navigation](/Pages/Documents/Navigation) documents for details on
configuring them).

#### Replacing Built-In Services

The **Configuration.ReplaceService** method can be used to **override** a
built-in service. For example, you can replace IAbpSession service with
your custom implementation as shown below:

    Configuration.ReplaceService<IAbpSession, MySession>(DependencyLifeStyle.Transient);

The ReplaceService method has an overload to pass an **action** to make a
replacement in a custom way (you can directly use Castle Windsor with its
advanced registration API).

The same service can be replaced multiple times, especially in different
modules. The last one replaced will be the one that is valid. The module PreInitialize
methods are executed by the [dependency order](Module-System.md).

### Configuring Modules

Besides the framework's own startup configuration, a module can extend
the **IAbpModuleConfigurations** interface to provide configuration points
for the module. Example:

    ...
    using Abp.Web.Configuration;
    ...
    public override void PreInitialize()
    {
        Configuration.Modules.AbpWebCommon().SendAllExceptionsToClients = true;
    }
    ...

In this example, we configured the AbpWebCommon module to send all
exceptions to clients.

Not every module should define this type of configuration. It's
generally needed when a module will be re-usable in different
applications and needs to be configured on startup.

### Creating Configuration For a Module

Assume that we have a module named MyModule and it has some
configuration properties. First, we create a class for these configurable
properties:

    public class MyModuleConfig
    {
        public bool SampleConfig1 { get; set; }

        public string SampleConfig2 { get; set; }
    }

We then register this class via [Dependency
Injection](Dependency-Injection.md) on the **PreInitialize** method of
MyModule (Thus, it will be injectable):

    IocManager.Register<MyModuleConfig>();

It should be registered as a **Singleton** like in this example. We can now
use the following code to configure MyModule in our module's
PreInitialize method:

    Configuration.Get<MyModuleConfig>().SampleConfig1 = false;

While we can use the IAbpStartupConfiguration.Get method as shown below, we
can create an extension method to the IModuleConfigurations like this:

    public static class MyModuleConfigurationExtensions
    {
        public static MyModuleConfig MyModule(this IModuleConfigurations moduleConfigurations)
        {
            return moduleConfigurations.AbpConfiguration.Get<MyModuleConfig>();
        }
    }

Now other modules can configure this module using the extension method:

    Configuration.Modules.MyModule().SampleConfig1 = false;
    Configuration.Modules.MyModule().SampleConfig2 = "test";

This makes it easy to investigate module configurations and collect them in
a single place (Configuration.Modules...). ABP itself defines extension
methods for its own module configurations.

At some point, MyModule needs this configuration. You can inject
MyModuleConfig and use the configured values. Example:

    public class MyService : ITransientDependency
    {
        private readonly MyModuleConfig _configuration;

        public MyService(MyModuleConfig configuration)
        {
            _configuration = configuration;
        }

        public void DoIt()
        {
            if (_configuration.SampleConfig2 == "test")
            {
                //...
            }
        }
    }

This way, modules can create central configuration points in the ASP.NET
Boilerplate system.
