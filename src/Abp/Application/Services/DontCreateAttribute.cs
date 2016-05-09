using System;
namespace Abp.Application.Services
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method)]
    public class DontCreateAttribute : Attribute
    {

    }
}
