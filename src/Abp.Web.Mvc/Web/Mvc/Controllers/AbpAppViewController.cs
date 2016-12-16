using System.Web.Mvc;
using Abp.Auditing;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Runtime.Validation;

namespace Abp.Web.Mvc.Controllers
{
    public class AbpAppViewController : AbpController
    {
        [DisableAuditing]
        [DisableValidation]
        [UnitOfWork(IsDisabled = true)]
        public ActionResult Load(string viewUrl)
        {
            return View(viewUrl.EnsureStartsWith('~'));
        }
    }
}
