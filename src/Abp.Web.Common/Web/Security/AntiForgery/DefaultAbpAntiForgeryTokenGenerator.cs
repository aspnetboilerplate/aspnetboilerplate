using System;
using Abp.Dependency;

namespace Abp.Web.Security.AntiForgery
{
    public class DefaultAbpAntiForgeryTokenGenerator : IAbpAntiForgeryTokenGenerator, ITransientDependency
    {
        public string Generate()
        {
            return Guid.NewGuid().ToString("D");
        }
    }
}