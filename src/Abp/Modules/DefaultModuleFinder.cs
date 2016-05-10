using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Reflection;

namespace Abp.Modules
{
    internal class DefaultModuleFinder : IModuleFinder
    {
        private readonly ITypeFinder _typeFinder;

        public DefaultModuleFinder(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        public ICollection<Type> FindAll()
        {
            return _typeFinder.Find(AbpModule.IsAbpModule).ToList();
        }
    }
}