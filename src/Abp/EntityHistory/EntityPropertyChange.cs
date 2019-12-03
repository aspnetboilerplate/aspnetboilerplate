using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Abp.EntityHistory
{
    [Table("AbpEntityPropertyChanges")]
    public class EntityPropertyChange : Entity<long>, IMayHaveTenant
    {
        /// <summary>
        /// Maximum length of <see cref="PropertyName"/> property.
        /// Value: 96.
        /// </summary>
        public const int MaxPropertyNameLength = 96;

        /// <summary>
        /// Maximum length of <see cref="NewValue"/> and <see cref="OriginalValue"/> properties.
        /// Value: 512.
        /// </summary>
        public const int MaxValueLength = 512;

        /// <summary>
        /// Maximum length of <see cref="PropertyTypeFullName"/> property.
        /// Value: 512.
        /// </summary>
        public const int MaxPropertyTypeFullNameLength = 192;

        /// <summary>
        /// EntityChangeId.
        /// </summary>
        public virtual long EntityChangeId { get; set; }

        /// <summary>
        /// NewValue.
        /// </summary>
        [StringLength(MaxValueLength)]
        public virtual string NewValue { get; set; }

        /// <summary>
        /// OriginalValue.
        /// </summary>
        [StringLength(MaxValueLength)]
        public virtual string OriginalValue { get; set; }

        /// <summary>
        /// PropertyName.
        /// </summary>
        [StringLength(MaxPropertyNameLength)]
        public virtual string PropertyName { get; set; }

        /// <summary>
        /// Type of the JSON serialized <see cref="NewValue"/> and <see cref="OriginalValue"/>.
        /// It's the FullName of the type.
        /// </summary>
        [StringLength(MaxPropertyTypeFullNameLength)]
        public virtual string PropertyTypeFullName { get; set; }

        /// <summary>
        /// TenantId.
        /// </summary>
        public virtual int? TenantId { get; set; }
    }
}
