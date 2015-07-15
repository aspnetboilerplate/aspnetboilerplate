using System;

namespace Abp.EntityFramework.Repositories
{
    /// <summary>
    /// Add this class to a DbContext to define auto-repository type for entities in this DbContext.
    /// This is useful if you inherit same DbContext by more than one DbContext.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoRepositoryTypeAttribute : Attribute
    {
        public Type RepositoryInterface { get; private set; }

        public Type RepositoryInterfaceWithPrimaryKey { get; private set; }
        
        public Type RepositoryImplementation { get; private set; }
        
        public Type RepositoryImplementationWithPrimaryKey { get; private set; }

        public AutoRepositoryTypeAttribute(Type repositoryInterface, Type repositoryInterfaceWithPrimaryKey, Type repositoryImplementation, Type repositoryImplementationWithPrimaryKey)
        {
            RepositoryInterface = repositoryInterface;
            RepositoryInterfaceWithPrimaryKey = repositoryInterfaceWithPrimaryKey;
            RepositoryImplementation = repositoryImplementation;
            RepositoryImplementationWithPrimaryKey = repositoryImplementationWithPrimaryKey;
        }
    }
}