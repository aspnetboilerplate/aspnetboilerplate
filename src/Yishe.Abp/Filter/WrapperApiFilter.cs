using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace Yishe.Abp.Filter
{
    public class WrapperApiAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);

            ApiResult result = new ApiResult();

            //// 取得由 API 返回的状态代码
            //result.Status = actionExecutedContext.ActionContext.Response.StatusCode;
            //// 取得由 API 返回的资料
            //result.Result = actionExecutedContext.ActionContext.Response.Content.ReadAsAsync<object>().Result;
            //// 重新封装回传格式
            //actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(result.Status, result);
        }
    }
}
