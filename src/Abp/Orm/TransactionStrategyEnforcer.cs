using Abp.Configuration.Startup;

namespace Abp
{
    /// <summary>
    ///     To enforce any type of TransactionStrategy using with <see cref="IAbpStartupConfiguration.ReplaceService" /> method.
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public delegate IAbpStartupConfiguration TransactionStrategyEnforcer(IAbpStartupConfiguration configuration);
}
