using System;
namespace Abp.Web.Modules
{
    public class AbpModuleAttribute : Attribute
    {
        public string Name { get; set; }

        public string[] Dependencies { get; set; }

        public AbpModuleAttribute(string name)
        {
            Name = name;
        }
    }
}