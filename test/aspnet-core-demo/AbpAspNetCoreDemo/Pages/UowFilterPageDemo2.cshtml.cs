using Abp.AspNetCore.Mvc.RazorPages;
using Abp.Domain.Uow;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;

namespace AbpAspNetCoreDemo.Pages
{
    [UnitOfWork(IsDisabled = true)]
    [IgnoreAntiforgeryToken]
    public class UowFilterPageDemo2 : AbpPageModel
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