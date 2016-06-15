using System;
using Abp.Domain.Repositories;

namespace Abp.EntityFramework.Repositories
{
    /// <summary>
    /// Add this class to a DbContext to define auto-repository types for entities in this DbContext.
    /// This is useful if you inherit same DbContext by more than one DbContext.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoRepositoryTypesAttribute : Attribute
    {
        public static AutoRepositoryTypesAttribute Default { get; private set; }

        public Type RepositoryInterface { get; private set; }

        public Type RepositoryInterfaceWithPrimaryKey { get; private set; }

        public Type RepositoryImplementation { get; private set; }

        public Type RepositoryImplementationWithPrimaryKey { get; private set; }

        static AutoRepositoryTypesAttribute()
        {
            Default = new AutoRepositoryTypesAttribute(
                typeof (IRepository<>),
                typeof (IRepository<,>),
                typeof (EfRepositoryBase<,>),
                typeof (EfRepositoryBase<,,>)
                );
        }

        public AutoRepositoryTypesAttribute(
            Type repositoryInterface, 
            Type repositoryInterfaceWithPrimaryKey, 
            Type repositoryImplementation, 
            Type repositoryImplementationWithPrimaryKey)
        {
            RepositoryInterface = repositoryInterface;
            RepositoryInterfaceWithPrimaryKey = repositoryInterfaceWithPrimaryKey;
            RepositoryImplementation = repositoryImplementation;
            RepositoryImplementationWithPrimaryKey = repositoryImplementationWithPrimaryKey;
        }
    }
}