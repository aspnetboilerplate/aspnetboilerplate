using Castle.Facilities.Logging;
using Abp.Dependency;
namespace Abp
{
    public static class Log4NetBootstrapper
    {
        public static void Start()
        {
            IocManager.Instance.IocContainer
                .AddFacility<LoggingFacility>(f => f.LogUsing<Log4NetFactory>()
              .WithConfig("log4net.config"));
        }
        
    }
}