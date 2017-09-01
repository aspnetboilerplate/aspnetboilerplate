#if NET46
using System.Configuration;
#endif

namespace Abp.Data
{
    public static class ConnectionStringHelper
    {
        /// <summary>
        /// Gets connection string from given connection string or name.
        /// </summary>
        public static string GetConnectionString(string nameOrConnectionString)
        {
#if NET46
            var connStrSection = ConfigurationManager.ConnectionStrings[nameOrConnectionString];
            if (connStrSection != null)
            {
                return connStrSection.ConnectionString;
            }
#endif

            return nameOrConnectionString;
        }
    }
}
