namespace Abp.Domain.Uow
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUnitOfWorkScopeManager
    {
        IUnitOfWork Current { get; set; }
    }
}