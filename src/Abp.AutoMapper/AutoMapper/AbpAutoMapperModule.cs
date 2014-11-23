using Abp.Modules;
using System.Reflection;
using Abp.Reflection;

namespace Abp.AutoMapper
{
    public class AbpAutoMapperModule : AbpModule
    {
        private readonly ITypeFinder _typeFinder;

        public AbpAutoMapperModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        public override void PreInitialize()
        {
            var types = _typeFinder.Find(type =>
                type.IsDefined(typeof (AutoMapAttribute)) ||
                type.IsDefined(typeof (AutoMapFromAttribute)) ||
                type.IsDefined(typeof (AutoMapToAttribute))
                );

            foreach (var type in types)
            {
                AutoMapperHelper.CreateMap(type);
            }
        }
    }
}
