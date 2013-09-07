using System.Reflection;

namespace Abp.Utils.Helpers
{
    /// <summary>
    /// This class is used to perform some common reflection related operations.
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// Gets a single attrbiute for a member.
        /// </summary>
        /// <typeparam name="T">Type of the attribute</typeparam>
        /// <param name="memberInfo">The member that will be checked for the attribute</param>
        /// <param name="inherit">Include inherited attrbiutes</param>
        /// <returns>Returns the attribute object if found. Returns null if not found.</returns>
        public static T GetSingleAttribute<T>(MemberInfo memberInfo, bool inherit = true) where T : class
        {
            var attrs = memberInfo.GetCustomAttributes(typeof(T), inherit);
            if (attrs.Length > 0)
            {
                return (T)attrs[0];
            }

            return default(T);
        }
    }
}