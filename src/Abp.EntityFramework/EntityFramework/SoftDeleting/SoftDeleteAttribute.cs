using System;

namespace Abp.EntityFramework.SoftDeleting
{
    /// <summary>
    ///     Attribute for soft delete.
    /// </summary>
    /// <remarks>
    ///     usage : [SoftDelete("YourColumnName")]
    /// </remarks>
    public class SoftDeleteAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the Abp.EntityFramework.SoftDeleting.SoftDeleteAttribute
        ///     class.
        /// </summary>
        /// <param name="column">The column.</param>
        public SoftDeleteAttribute(string column)
        {
            this.ColumnName = column;
        }

        /// <summary>
        ///     Gets or sets the name of the column.
        /// </summary>
        /// <value>
        ///     The name of the column.
        /// </value>
        public string ColumnName { get; set; }
    }
}