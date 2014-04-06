using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using Abp.Dependency;
using Abp.Domain.Entities;

namespace Abp.Domain.Repositories.EntityFramework
{
    public class AbpDbContext : DbContext, ITransientDependency
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
                    .Where(type => (!type.IsAbstract && IsAssignableToGenericType(type, typeof(IEntity<>))))
                    .ToList();

                foreach (var entityType in entityTypes)
                {
                    var method = modelBuilder.GetType().GetMethod("Entity");
                    method = method.MakeGenericMethod(new[] { entityType });
                    method.Invoke(modelBuilder, null);
                }
            }
        }

        private static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = givenType.BaseType;
            if (baseType == null) return false;

            return IsAssignableToGenericType(baseType, genericType);
        }
    }
}
