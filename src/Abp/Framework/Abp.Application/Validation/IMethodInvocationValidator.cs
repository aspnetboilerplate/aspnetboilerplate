using System.Reflection;

namespace Abp.Validation
{
    public interface IMethodInvocationValidator
    {
        void Validate(object targetObject, MethodInfo method, object[] arguments);
    }
}