using Castle.Facilities.Logging;

namespace Abp.Castle.Logging.Log4Net
{
    public static class LoggingFacilityExtensions
    {
        public static LoggingFacility UseAbpLog4Net(this LoggingFacility loggingFacility)
        {
            return loggingFacility.LogUsing<Log4NetLoggerFactory>();
        }
    }
}