using System;
using Abp.Dependency;

namespace Abp.Web.Security
{
    public class DefaultCsrfTokenGenerator : ICsrfTokenGenerator, ITransientDependency
    {
        public string Generate()
        {
            return Guid.NewGuid().ToString("D");
        }
    }
}