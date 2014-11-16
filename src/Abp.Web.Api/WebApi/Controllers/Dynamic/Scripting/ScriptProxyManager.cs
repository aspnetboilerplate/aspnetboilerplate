using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Abp.Dependency;
using Abp.WebApi.Controllers.Dynamic.Scripting.Angular;
using Abp.WebApi.Controllers.Dynamic.Scripting.jQuery;

namespace Abp.WebApi.Controllers.Dynamic.Scripting
{
    //TODO@Halil: This class can be optimized.
    public class ScriptProxyManager : ISingletonDependency
    {
        private readonly IDictionary<string, ScriptInfo> CachedScripts;

        public ScriptProxyManager()
        {
            CachedScripts = new Dictionary<string, ScriptInfo>();
        }

        public string GetScript(string name, ProxyScriptType type)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("name is null or empty!", "name");
            }

            GenerateScriptsIfNeeded(type);

            var cacheKey = type + "_" + name;
            lock (CachedScripts)
            {
                if (!CachedScripts.ContainsKey(cacheKey))
                {
                    throw new HttpException(404, "There is no such a service: " + cacheKey);
                }

                return CachedScripts[cacheKey].Script;
            }
        }

        public string GetAllScript(ProxyScriptType type)
        {
            lock (CachedScripts)
            {
                var cacheKey = type + "_all";
                if (!CachedScripts.ContainsKey(cacheKey))
                {
                    var script = new StringBuilder();

                    var dynamicControllers = DynamicApiControllerManager.GetAll();
                    foreach (var dynamicController in dynamicControllers)
                    {
                        var proxyGenerator = CreateProxyGenerator(type, dynamicController, false);
                        script.AppendLine(proxyGenerator.Generate());
                        script.AppendLine();
                    }

                    CachedScripts[cacheKey] = new ScriptInfo(script.ToString());
                }

                return CachedScripts[cacheKey].Script;
            }
        }

        public void GenerateScriptsIfNeeded(ProxyScriptType type)
        {
            lock (CachedScripts)
            {
                if (CachedScripts.Count > 0)
                {
                    return;
                }

                var dynamicControllers = DynamicApiControllerManager.GetAll();
                foreach (var dynamicController in dynamicControllers)
                {
                    var proxyGenerator = CreateProxyGenerator(type, dynamicController, true);
                    var script = proxyGenerator.Generate();
                    var cacheKey = type + "_" + dynamicController.ServiceName;
                    CachedScripts[cacheKey] = new ScriptInfo(script);
                }
            }
        }

        private static IScriptProxyGenerator CreateProxyGenerator(ProxyScriptType type, DynamicApiControllerInfo controllerInfo, bool amdModule)
        {
            switch (type)
            {
                case ProxyScriptType.JQuery:
                    return new JQueryProxyGenerator(controllerInfo, amdModule);
                case ProxyScriptType.Angular:
                    return new AngularProxyGenerator(controllerInfo);
                default:
                    throw new AbpException("Unknown ProxyScriptType: " + type);
            }
        }

        private class ScriptInfo
        {
            public string Script { get; private set; }

            public ScriptInfo(string script)
            {
                Script = script;
            }
        }
    }
}
