using Castle.Windsor;

namespace Abp.Modules
{
    public interface IAbpInitializationContext
    {
        WindsorContainer IocContainer { get; }

        TModule GetModule<TModule>() where TModule : AbpModule;
    }
}