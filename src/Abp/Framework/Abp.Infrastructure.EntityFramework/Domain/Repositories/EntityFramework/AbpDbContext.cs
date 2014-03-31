using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Exceptions;

namespace Abp.Domain.Repositories.EntityFramework
{
    public abstract class AbpDbContext : DbContext, ITransientDependency
    {
        private static readonly List<Assembly> _entityAssemblies = new List<Assembly>();

        protected AbpDbContext()
        {

        }

        protected AbpDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        public static void AddEntityAssembly(Assembly assembly)
        {
            if (_entityAssemblies.Contains(assembly))
            {
                throw new AbpException("This assembly is already added: " + assembly.FullName);
            }

            _entityAssemblies.Add(assembly);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityAssembly in _entityAssemblies)
            {
                var entityTypes = entityAssembly.GetTypes()
                    .Where(type => (!type.IsAbstract && (typeof(IEntity<>).IsAssignableFrom(type))))
                    .ToList();

                foreach (var entityType in entityTypes)
                {
                    var method = modelBuilder.GetType().GetMethod("Entity");
                    method = method.MakeGenericMethod(new[] { entityType });
                    method.Invoke(modelBuilder, null);
                }
            }
        }
    }
}
