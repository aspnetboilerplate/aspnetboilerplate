using System.Linq;
using System.Reflection;

namespace Abp.Reflection
{
    public static class CastleProxyHelper
    {
        public static object GetProxyTargetOrNull(object repository)
        {
            if (repository.GetType().Namespace != "Castle.Proxies")
            {
                return null;
            }

            var targetField = repository.GetType().GetTypeInfo()
                .GetFields()
                .FirstOrDefault(f => f.Name == "__target");

            if (targetField == null)
            {
                return null;
            }

            return targetField.GetValue(repository);
        }
    }
}
