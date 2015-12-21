using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Threading;

namespace Abp.Swagger
{
   public static  class SwaggerBuilder
    {
       static string UrlTemplate = "api/services/{servicePrefix}/{controller}/{action}/{idl}";

     public  static IDictionary<string, string> ContentJson = new Dictionary<string, string>();
       /// <summary>
       /// 针对程序集获取Swagger信息
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <param name="assembly"></param>
       /// <param name="servicePrefix"></param>
        public static  void ForAll<T>(Assembly assembly, string servicePrefix)
     {
         var swagpath = HttpContext.Current != null ? HttpContext.Current.Server.MapPath("~/Swag/")+servicePrefix+"/" : "";
            Task.Factory.StartNew(() =>
             {
            var generator = new AssemblyTypeToSwaggerGenerator(assembly.CodeBase);
            var allClassNames = generator.GetAbpServiceBaseClasses();

            Parallel.ForEach<string>(allClassNames, item => GenJsonForOneClass(item,UrlTemplate.Replace("{servicePrefix}",servicePrefix),swagpath,generator));

                       
            });
              
        }

       /// <summary>
       /// 针对当个接口获取Swagger信息
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <param name="assembly"></param>
       /// <param name="servicePrefix"></param>
       /// <param name="serviceName"></param>
        public static void For<T>(Assembly assembly, string servicePrefix, string serviceName) 
        {
            var swagpath = HttpContext.Current != null ? HttpContext.Current.Server.MapPath("~/Swag/") + servicePrefix + "/" : "";   
            var generator = new AssemblyTypeToSwaggerGenerator(assembly.CodeBase);
            var typename = generator.GetAbpServiceBaseClassByInterface(typeof(T).FullName);
            GenJsonForOneClass(typename, UrlTemplate.Replace("{servicePrefix}", servicePrefix), swagpath, generator,serviceName);

        }



        static void GenJsonForOneClass(string classname, string url, string swagpath, AssemblyTypeToSwaggerGenerator generator, string controllernameused=null)
        {
            var swobj = generator.FromAbpApplicationMoudleAssembly(classname, url,controllernameused);
            if (swobj != null)
            {
                //gen json file 
                var jsontext = swobj.ToJson();
                if (!string.IsNullOrEmpty(swagpath))
                {
                    System.IO.Directory.CreateDirectory(swagpath);
                    //取得类名
                    var clsplit=classname.Split('.');
                    //去除AppSerivce
                    if (clsplit!=null&&clsplit.Length>0)
                    {
                        classname = clsplit[clsplit.Length - 1];
                        classname = classname.Replace("AppService", "");
                        classname = classname.Replace("Service", "");
                    }
                    if (!string.IsNullOrEmpty(controllernameused))
                    {
                        classname = controllernameused;
                    }
                    var file = swagpath + "\\" + classname + ".js";
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }
                    var writer = File.CreateText(file);
                    writer.Write(jsontext);
                    writer.Close();

                }
            }
        }
    }
}
