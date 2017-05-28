using System.Linq;

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

            var targetField = repository.GetType()
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
