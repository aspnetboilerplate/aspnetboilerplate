using System;

namespace Abp.Domain.Repositories
{
    /// <summary>
    /// Used to define auto-repository types for entities.
    /// This can be used for DbContext types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoRepositoryTypesAttribute : Attribute
    {
        public Type RepositoryInterface { get; }

        public Type RepositoryInterfaceWithPrimaryKey { get; }

        public Type RepositoryImplementation { get; }

        public Type RepositoryImplementationWithPrimaryKey { get; }

        public bool WithDefaultRepositoryInterfaces { get; set; }

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