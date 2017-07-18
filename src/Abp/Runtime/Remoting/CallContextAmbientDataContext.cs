#if NET46
using System.Runtime.Remoting.Messaging;
using Abp.Dependency;

namespace Abp.Runtime.Remoting
{
    public class CallContextAmbientDataContext : IAmbientDataContext, ISingletonDependency
    {
        public void SetData(string key, object value)
        {
            if (value == null)
            {
                CallContext.FreeNamedDataSlot(key);
                return;
            }

            CallContext.LogicalSetData(key, value);
        }

        public object GetData(string key)
        {
            return CallContext.LogicalGetData(key);
        }
    }
}
#endif