using Abp.Modules;
using System.Reflection;
using Abp.Reflection;
using Castle.Core.Logging;

namespace Abp.AutoMapper
{
    public class AbpAutoMapperModule : AbpModule
    {
        public ILogger Logger { get; set; }

        private readonly ITypeFinder _typeFinder;

        public AbpAutoMapperModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
            Logger = NullLogger.Instance;
        }

        public override void PreInitialize()
        {
            var types = _typeFinder.Find(type =>
                type.IsDefined(typeof (AutoMapAttribute)) ||
                type.IsDefined(typeof (AutoMapFromAttribute)) ||
                type.IsDefined(typeof (AutoMapToAttribute))
                );

            Logger.DebugFormat("Found {0} classes defines auto mapping attributes", types.Length);
            foreach (var type in types)
            {
                Logger.Debug(type.FullName);
                AutoMapperHelper.CreateMap(type);
            }
        }
    }
}
