using System;
using System.Collections.Generic;
using Abp.Collections;
using Abp.Modules;

namespace Abp.TestBase.Modules
{
    public class TestModuleFinder : IModuleFinder
    {
        public TestModuleFinder()
        {
            Modules = new TypeList<AbpModule>();
        }

        public ITypeList<AbpModule> Modules { get; }

        public ICollection<Type> FindAll()
        {
            return Modules;
        }
    }
}