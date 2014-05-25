using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Abp.WebApi.Controllers.Dynamic.Scripting
{
    //TODO@Halil: This class can be optimized.
    internal static class ScriptProxyManager
    {
        private static readonly IDictionary<string, ScriptInfo> Scripts;

        private static string _allServicesScript;

        static ScriptProxyManager()
        {
            Scripts = new Dictionary<string, ScriptInfo>();
        }

        public static string GetScript(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("name is null or empty!", "name");
            }

            GenerateScriptsIfNeeded();

            lock (Scripts)
            {
                if (!Scripts.ContainsKey(name))
                {
                    throw new HttpException(404, "There is no such a service: " + name);
                }

                return Scripts[name].Script;
            }
        }

        public static string GetAllScript()
        {
            lock (Scripts)
            {
                if (_allServicesScript == null)
                {
                    var script = new StringBuilder();

                    var dynamicControllers = DynamicApiControllerManager.GetAll();
                    foreach (var dynamicController in dynamicControllers)
                    {
                        script.AppendLine(new ControllerScriptProxyGenerator().GenerateFor(dynamicController, false));
                        script.AppendLine();
                    }

                    _allServicesScript = script.ToString();
                }

                return _allServicesScript;
            }
        }

        public static void GenerateScriptsIfNeeded()
        {
            lock (Scripts)
            {
                if (Scripts.Count > 0)
                {
                    return;
                }

                var dynamicControllers = DynamicApiControllerManager.GetAll();
                foreach (var dynamicController in dynamicControllers)
                {
                    var script = new ControllerScriptProxyGenerator().GenerateFor(dynamicController);
                    Scripts[dynamicController.ServiceName] = new ScriptInfo(dynamicController.ServiceName, script);
                }
            }
        }

        private class ScriptInfo
        {
            public string ServiceName { get; private set; }

            public string Script { get; private set; }

            public ScriptInfo(string serviceName, string script)
            {
                ServiceName = serviceName;
                Script = script;
            }
        }
    }
}
