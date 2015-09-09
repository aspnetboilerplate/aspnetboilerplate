using System.Reflection;
using Abp.Modules;

namespace MyProject
{
    /// <summary>
    /// 'Application layer module' for this project.
    /// </summary>
    [DependsOn(typeof(MyProjectCoreModule))]
    public class MyProjectApplicationModule : AbpModule
    {
        public override void Initialize()
        {
            //This code is used to register classes to dependency injection system for this assembly using conventions.
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            //We must declare mappings to be able to use AutoMapper
            DtoMappings.Map();
        }
    }
}
