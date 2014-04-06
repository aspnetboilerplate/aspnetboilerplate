using Abp.Dependency;
using Castle.Core.Logging;

namespace Abp.Logging
{
    /// <summary>
    /// This class can be used to write logs from somewhere where it's a little hard to get a reference to the <see cref="ILogger"/>.
    /// Normally, get <see cref="ILogger"/> using property injection.
    /// TODO: Remove this class
    /// </summary>
    internal class LogHelper
    {
        /// <summary>
        /// A reference to the logger.
        /// </summary>
        public static ILogger Logger { get; private set; }

        static LogHelper()
        {
            Logger = IocHelper.Resolve<ILogger>();
        }
    }
}
