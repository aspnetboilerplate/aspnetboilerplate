using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;

namespace Abp.Swagger
{
   public static  class SwaggerBuilder
    {
       static string UrlTemplate = "api/services/{servicePrefix}/{controller}/{action}/{idl}";

     public  static IDictionary<string, string> ContentJson = new Dictionary<string, string>();
        public static void ForAll<T>(Assembly assembly, string servicePrefix)
        {
            var generator = new AssemblyTypeToSwaggerGenerator(assembly.CodeBase);
            var allClassNames = generator.GetAbpServiceBaseClasses();
            foreach (var item in allClassNames)
            {
                var jsontext = generator.FromAbpApplicationMoudleAssembly(item, UrlTemplate.Replace("{servicePrefix}",servicePrefix)).ToJson();
                //gen json file 
                if (HttpContext.Current!=null)
	            {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Swag/"));
                    var file = HttpContext.Current.Server.MapPath("~/Swag/")+"/"+item+".js";
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }
                    var writer=File.CreateText(file);
                    writer.Write(jsontext);
                    writer.Close();
		 
	            }
              
            }
              
        }
    }
}
