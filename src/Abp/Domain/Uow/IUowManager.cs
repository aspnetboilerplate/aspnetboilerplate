namespace Abp.Domain.Uow
{
    /// <summary>
    /// Unit of work manager.
    /// </summary>
    public interface IUowManager
    {
        IActiveUnitOfWork Current { get; }

        IUnitOfWorkCompleteHandle StartNew(bool isTransactional = true);
    }
}
