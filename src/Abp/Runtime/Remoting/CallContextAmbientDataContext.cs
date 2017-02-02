using System.Runtime.Remoting.Messaging;
using Abp.Dependency;

namespace Abp.Runtime.Remoting
{
    //TODO: We can switch to AsyncLocal once we support .Net 4.6.1.
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