using Abp.WebApi.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Abp.Web.Api.Swagger.Controller
{
    public class SwaggerInfoController : AbpApiController
    {
       



       [HttpGet]
       public IDictionary<string,IList<string>> GetMoudles() 
       {
           var swagpath = HttpContext.Current != null ? HttpContext.Current.Server.MapPath("~/Swag/") : "";
           IDictionary<string, IList<string>> _class = new Dictionary<string, IList<string>>();
           var dirs = Directory.GetDirectories(swagpath);
         
           foreach (var item in dirs)
           {

               var itemstr = item.Split('\\').LastOrDefault();
               _class.Add(itemstr,new List<string>());
               var files = Directory.GetFiles(swagpath + itemstr);
               foreach (var itemf in files)
               {
                   var itemfstr = itemf.Split('\\').LastOrDefault();
                   _class[itemstr].Add(itemfstr.Replace(".js",""));
               }
               
           }

           return _class;
       }
    }
}
