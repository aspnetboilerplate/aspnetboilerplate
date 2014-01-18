namespace Abp.WebApi.Controllers.Dynamic.Scripting
{
    internal class ScriptInfo
    {
        public string ServiceName { get; set; }

        public string Script { get; set; }

        public ScriptInfo(string serviceName, string script)
        {
            ServiceName = serviceName;
            Script = script;
        }
    }
}