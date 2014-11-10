using System;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Reflection;
using Abp.Collections.Extensions;
using Abp.EntityFramework.Dependency;
using Abp.EntityFramework.Repositories;
using Abp.EntityFramework.SoftDeleting;
using Abp.Modules;
using Abp.Reflection;

namespace Abp.EntityFramework
{
    /// <summary>
    /// This module is used to implement "Data Access Layer" in EntityFramework.
    /// </summary>
    public class AbpEntityFrameworkModule : AbpModule
    {
        public IAssemblyFinder AssemblyFinder { private get; set; }

        public AbpEntityFrameworkModule()
        {
            AssemblyFinder = DefaultAssemblyFinder.Instance;
        }

        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new EntityFrameworkConventionalRegisterer());
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            DbInterception.Add(new SoftDeleteInterceptor());
            RegisterGenericRepositories();
        }

        private void RegisterGenericRepositories()
        {
            //TODO: Refactor this code. Also, it's similar to DefaultModuleFinder.FindAll

            var allAssemblies = AssemblyFinder.GetAllAssemblies().Distinct();

            foreach (var assembly in allAssemblies)
            {
                Type[] types;

                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = ex.Types;
                }

                if (types.IsNullOrEmpty())
                {
                    continue;
                }

                var dbContextTypes = (
                    from type in types
                    where
                        type.IsPublic && !type.IsAbstract && type.IsClass &&
                        typeof (AbpDbContext).IsAssignableFrom(type)
                    select type
                    ).ToArray();

                if (dbContextTypes.IsNullOrEmpty())
                {
                    continue;
                }

                foreach (var dbContextType in dbContextTypes)
                {
                    EntityFrameworkGenericRepositoryRegistrar.RegisterDbContext(dbContextType, IocManager);
                }
            }
        }
    }
}
