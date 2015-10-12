using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace Yishe.Abp.Filter
{
    public class ClaimsPrincipalConvertUseSubClaimsToThreadPrincipalFilter : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {

            var caller = actionContext.RequestContext.Principal as ClaimsPrincipal;
            if (caller!=null)
            {
                var subjectClaim = caller.FindFirst("sub");
                if (subjectClaim !=null)
                {
                    string userid = subjectClaim.Value;
                    Thread.CurrentPrincipal =new GenericPrincipal(new GenericIdentity(userid),null);
                }
            }

           
            


            base.OnActionExecuting(actionContext);
        }
    }
}
