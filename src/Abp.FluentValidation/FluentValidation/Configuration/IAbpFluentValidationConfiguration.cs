namespace Abp.FluentValidation.Configuration
{
    public interface IAbpFluentValidationConfiguration
    {
        /// <summary>
        /// Name of the source name to get translations from.
        /// </summary>
        string LocalizationSourceName { get; set; }
    }
}
