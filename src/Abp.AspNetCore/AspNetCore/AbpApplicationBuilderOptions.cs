namespace Abp.AspNetCore
{
    public class AbpApplicationBuilderOptions
    {
        /// <summary>
        /// Default: true.
        /// </summary>
        public bool UseCastleLoggerFactory { get; set; }

        /// <summary>
        /// Default: true.
        /// </summary>
        public bool UseAbpRequestLocalization { get; set; }

        /// <summary>
        /// Default: true.
        /// </summary>
        public bool AddSecurityHeaders { get; set; }

        public AbpApplicationBuilderOptions()
        {
            UseCastleLoggerFactory = true;
            UseAbpRequestLocalization = true;
            AddSecurityHeaders = true;
        }
    }
}