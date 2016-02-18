namespace Abp.Logging
{
    /// <summary>
    /// Interface to define a <see cref="Severity"/> property (see <see cref="LogSeverity"/>).
    /// </summary>
    public interface IHasLogSeverity
    {
        /// <summary>
        /// Log severity.
        /// </summary>
        LogSeverity Severity { get; set; }
    }
}