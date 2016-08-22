using System;

namespace Abp.Web.Security.AntiForgery
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method)]
    public class ValidateAbpAntiForgeryTokenAttribute : Attribute
    {

    }
}