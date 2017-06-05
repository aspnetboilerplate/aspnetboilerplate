using System;

using Abp.Domain.Repositories;

using JetBrains.Annotations;

namespace Abp.DapperCore
{
    public class DapperCoreAutoRepositoryTypeAttribute : AutoRepositoryTypesAttribute
    {
        public DapperCoreAutoRepositoryTypeAttribute(
            [NotNull] Type repositoryInterface,
            [NotNull] Type repositoryInterfaceWithPrimaryKey,
            [NotNull] Type repositoryImplementation,
            [NotNull] Type repositoryImplementationWithPrimaryKey)
            : base(repositoryInterface, repositoryInterfaceWithPrimaryKey, repositoryImplementation, repositoryImplementationWithPrimaryKey)
        {
        }
    }
}
