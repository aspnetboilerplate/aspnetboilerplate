using System;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.EntityFramework.Repositories;
using Abp.Tests;
using Castle.MicroKernel.Registration;
using Shouldly;
using Xunit;

namespace Abp.EntityFramework.Tests.Repositories
{
    public class Generic_Repository_Tests : TestBaseWithSelfIocManager
    {
        [Fact]
        public void Should_Resolve_Generic_Repositories()
        {
            EfGenericRepositoryRegistrar.Register<MyDbContext>(LocalIocManager);

            var repository1 = LocalIocManager.Resolve<IRepository<MyEntity1>>();
            repository1.ShouldNotBe(null);

            var repository2 = LocalIocManager.Resolve<IRepository<MyEntity2, long>>();
            repository2.ShouldNotBe(null);
        }

        public class MyDbContext : AbpDbContext
        {
            public IDbSet<MyEntity1> MyEntities1 { get; set; }

            public DbSet<MyEntity2> MyEntities2 { get; set; }
        }

        public class MyEntity1 : Entity
        {

        }

        public class MyEntity2 : Entity<long>
        {

        }
    }

    public static class EfGenericRepositoryRegistrar
    {
        public static void Register<TDbContext>(IIocManager iocManager) where TDbContext : AbpDbContext
        {
            var dbSetProperties =
                from property in typeof(TDbContext).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                where
                    IsAssignableToGenericType(property.PropertyType, typeof(IDbSet<>)) ||
                    IsAssignableToGenericType(property.PropertyType, typeof(DbSet<>))
                select property;

            foreach (var dbSetProperty in dbSetProperties)
            {
                if (dbSetProperty.PropertyType.GenericTypeArguments.Length == 1)
                {
                    var entityType = dbSetProperty.PropertyType.GenericTypeArguments[0];

                    var inters = entityType.GetInterfaces();
                    foreach (var inter in inters)
                    {
                        if (inter == typeof(IEntity))
                        {
                            var repoInterType = typeof(IRepository<>).MakeGenericType(entityType);
                            var repoImplType = typeof(EfRepositoryBase<,>).MakeGenericType(typeof(TDbContext), entityType);

                            iocManager.IocContainer.Register(
                                Component.For(repoInterType).ImplementedBy(repoImplType)
                                );
                        }
                        else //TODO: Burası tamamlanmadı!!!
                        {
                            var pkType = inter.GenericTypeArguments[0];
                            var repoInterType = typeof(IRepository<,>).MakeGenericType(entityType, pkType);
                            var repoImplType = typeof(EfRepositoryBase<,,>).MakeGenericType(typeof(TDbContext), entityType, pkType);

                            iocManager.IocContainer.Register(
                                Component.For(repoInterType).ImplementedBy(repoImplType)
                                );
                        }
                    }
                }
            }
        }

        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
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
