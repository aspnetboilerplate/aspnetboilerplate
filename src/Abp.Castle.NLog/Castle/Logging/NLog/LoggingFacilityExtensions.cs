using Castle.Facilities.Logging;

namespace Abp.Castle.Logging.NLog
{
    public static class LoggingFacilityExtensions
    {
        public static LoggingFacility UseAbpNLog(this LoggingFacility loggingFacility)
        {
            return loggingFacility.LogUsing<NLogLoggerFactory>();
        }
    }
}
