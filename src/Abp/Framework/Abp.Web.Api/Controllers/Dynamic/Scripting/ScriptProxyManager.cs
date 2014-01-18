using System;
using System.Collections.Generic;
using System.Web;
using Abp.Utils.Extensions;

namespace Abp.WebApi.Controllers.Dynamic.Scripting
{
    internal static class ScriptProxyManager
    {
        private static readonly IDictionary<string, ScriptInfo> Scripts;

        static ScriptProxyManager()
        {
            Scripts = new Dictionary<string, ScriptInfo>();
        }

        public static string GetScript(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || !name.Contains("/"))
            {
                throw new ArgumentException("name must be formatted as {areaName}/{serviceName}", "name");
            }

            lock (Scripts)
            {
                //TODO: Use reader writer lock for performance reasons
                if (Scripts.ContainsKey(name))
                {
                    return Scripts[name].Script;
                }

                var controllerInfo = DynamicApiControllerManager.Find(name.ToPascalCase());
                if (controllerInfo == null)
                {
                    throw new HttpException(404, "There is no such a service: " + name);
                }

                var script = new ControllerScriptProxyGenerator().GenerateFor(controllerInfo);
                Scripts[name] = new ScriptInfo(name, script);
                return script;
            }
        }
    }
}
