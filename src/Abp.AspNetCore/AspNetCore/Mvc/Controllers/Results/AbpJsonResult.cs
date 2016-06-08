using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

/* This class is inspired from http://www.matskarlsson.se/blog/serialize-net-objects-as-camelcase-json */

namespace Abp.AspNetCore.Mvc.Controllers.Results
{
    /// <summary>
    /// This class is used to override returning Json results from MVC controllers.
    /// </summary>
    public class AbpJsonResult : JsonResult
    {
        /// <summary>
        /// Constructor with JSON data.
        /// </summary>
        public AbpJsonResult(object value)
            : base(value)
        {
            //JsonRequestBehavior = JsonRequestBehavior.DenyGet;
        }

        /// <summary>
        /// Constructor with JSON data.
        /// </summary>
        public AbpJsonResult(object value, JsonSerializerSettings serializerSettings)
            : base(value, serializerSettings)
        {
            //JsonRequestBehavior = JsonRequestBehavior.DenyGet;
        }

        //public override void ExecuteResult(ActionContext context)
        //{
        //    throw new NotImplementedException();

        //    if (context == null)
        //    {
        //        throw new ArgumentNullException("context");
        //    }

        //    var ignoreJsonRequestBehaviorDenyGet = false;
        //    if (context.HttpContext.Items.Contains("IgnoreJsonRequestBehaviorDenyGet"))
        //    {
        //        ignoreJsonRequestBehaviorDenyGet = String.Equals(context.HttpContext.Items["IgnoreJsonRequestBehaviorDenyGet"].ToString(), "true", StringComparison.OrdinalIgnoreCase);
        //    }

        //    if (!ignoreJsonRequestBehaviorDenyGet && JsonRequestBehavior == JsonRequestBehavior.DenyGet && String.Equals(context.HttpContext.Request.Method, "GET", StringComparison.OrdinalIgnoreCase))
        //    {
        //        throw new InvalidOperationException("This request has been blocked because sensitive information could be disclosed to third party web sites when this is used in a GET request. To allow GET requests, set JsonRequestBehavior to AllowGet.");
        //    }

        //    var response = context.HttpContext.Response;

        //    response.ContentType = !String.IsNullOrEmpty(ContentType) ? ContentType : "application/json";
        //    if (ContentEncoding != null)
        //    {
        //        response.ContentEncoding = ContentEncoding;
        //    }

        //    if (Value != null)
        //    {
        //        response.Write(Data.ToJsonString(true, true));
        //    }
        //}
    }
}