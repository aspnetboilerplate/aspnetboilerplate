using System;
using System.Collections.Generic;
using Abp.Collections;
using Abp.Modules;

namespace Abp.TestBase.Modules
{
    public class TestModuleFinder : IModuleFinder
    {
        public ITypeList<AbpModule> Modules { get; private set; }

        public TestModuleFinder()
        {
            Modules = new TypeList<AbpModule>();
        }

        public ICollection<Type> FindAll()
        {
            return Modules;
        }
    }
}