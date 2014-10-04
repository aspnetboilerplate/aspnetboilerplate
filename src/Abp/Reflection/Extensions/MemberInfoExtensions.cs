using System;
using System.Reflection;

namespace Abp.Reflection.Extensions
{
    /// <summary>
    /// Extensions to <see cref="MemberInfo"/>.
    /// </summary>
    public static class MemberInfoExtensions
    {
        /// <summary>
        /// Gets a single attribute for a member.
        /// </summary>
        /// <typeparam name="T">Type of the attribute</typeparam>
        /// <param name="memberInfo">The member that will be checked for the attribute</param>
        /// <param name="inherit">Include inherited attributes</param>
        /// <returns>Returns the attribute object if found. Returns null if not found.</returns>
        public static T GetSingleAttributeOrNull<T>(this MemberInfo memberInfo, bool inherit = true) where T : class
        {
            if (memberInfo == null)
            {
                throw new ArgumentNullException("memberInfo");
            }

            var attrs = memberInfo.GetCustomAttributes(typeof(T), inherit);
            if (attrs.Length > 0)
            {
                return (T)attrs[0];
            }

            return default(T);
        }
    }
}
