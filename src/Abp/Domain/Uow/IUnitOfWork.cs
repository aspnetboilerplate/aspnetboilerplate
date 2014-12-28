using Abp.Dependency;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// Defines a unit of work.
    /// </summary>
    public interface IUnitOfWork : IActiveUnitOfWork, IUnitOfWorkCompleteHandle, ITransientDependency
    {
        void Start();

        void Start(UnitOfWorkOptions options);
    }
}