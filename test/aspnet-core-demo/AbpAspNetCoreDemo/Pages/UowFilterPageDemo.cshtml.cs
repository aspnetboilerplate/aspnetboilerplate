using Abp.Domain.Uow;
using Abp.UI;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AbpAspNetCoreDemo.Pages
{
    public class UowFilterPageDemo : PageModel
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