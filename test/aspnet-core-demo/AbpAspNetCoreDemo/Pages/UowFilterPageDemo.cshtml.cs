using Abp.AspNetCore.Mvc.RazorPages;
using Abp.Domain.Uow;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;

namespace AbpAspNetCoreDemo.Pages
{
    [IgnoreAntiforgeryToken]
    public class UowFilterPageDemo : AbpPageModel
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UowFilterPageDemo(IUnitOfWorkManager unitOfWorkManager)
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

        [UnitOfWork(IsDisabled = true)]
        public void OnPost()
        {
            if (_unitOfWorkManager.Current == null)
            {
                throw new UserFriendlyException("Current UnitOfWork is null");
            }
        }
    }
}