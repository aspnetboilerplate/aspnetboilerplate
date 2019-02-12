using Abp.Domain.Uow;
using Abp.UI;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AbpAspNetCoreDemo.Pages
{
    [UnitOfWork(IsDisabled = true)]
    public class UowFilterPageDemo2 : PageModel
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UowFilterPageDemo2(IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        public void OnGet()
        {
            if (_unitOfWorkManager.Current == null)
            {
                throw new UserFriendlyException("Current UnitOfWork is null");
            }
        }

        public void OnPost()
        {
            if (_unitOfWorkManager.Current == null)
            {
                throw new UserFriendlyException("Current UnitOfWork is null");
            }
        }
    }
}