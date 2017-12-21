namespace Abp.EntityHistory
{
    /// <summary>
    /// EntityChangeType.
    /// </summary>
    public enum EntityChangeType : byte
    {
        /// <summary>
        /// Created.
        /// </summary>
        Created = 0,

        /// <summary>
        /// Deleted.
        /// </summary>
        Deleted = 1,

        /// <summary>
        /// Modified.
        /// </summary>
        Modified = 2
    }
}
