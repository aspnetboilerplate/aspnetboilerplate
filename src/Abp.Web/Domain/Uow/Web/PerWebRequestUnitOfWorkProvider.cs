namespace Abp.Domain.Uow.Web
{
    public class PerWebRequestUnitOfWorkProvider : ICurrentUnitOfWorkProvider
    {
        public IUnitOfWork Current { get; set; }
    }
}