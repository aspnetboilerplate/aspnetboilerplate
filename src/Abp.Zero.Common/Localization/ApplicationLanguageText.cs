using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Abp.Localization
{
    /// <summary>
    /// Used to store a localization text.
    /// </summary>
    [Serializable]
    [Table("AbpLanguageTexts")]
    public class ApplicationLanguageText : AuditedEntity<long>, IMayHaveTenant
    {
        public const int MaxSourceNameLength = 128;
        public const int MaxKeyLength = 256;
        public const int MaxValueLength = 64 * 1024 * 1024; //64KB

        /// <summary>
        /// TenantId of this entity. Can be null for host.
        /// </summary>
        public virtual int? TenantId { get; set; }

        /// <summary>
        /// Language name (culture name). Matches to <see cref="ApplicationLanguage.Name"/>.
        /// </summary>
        [Required]
        [StringLength(ApplicationLanguage.MaxNameLength)]
        public virtual string LanguageName { get; set; }

        /// <summary>
        /// Localization source name
        /// </summary>
        [Required]
        [StringLength(MaxSourceNameLength)]
        public virtual string Source { get; set; }

        /// <summary>
        /// Localization key
        /// </summary>
        [Required]
        [StringLength(MaxKeyLength)]
        public virtual string Key { get; set; }

        /// <summary>
        /// Localized value
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        [StringLength(MaxValueLength)]
        public virtual string Value { get; set; }
    }
}