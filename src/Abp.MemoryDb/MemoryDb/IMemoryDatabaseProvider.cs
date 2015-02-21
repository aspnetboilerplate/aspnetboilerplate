namespace Abp.MemoryDb
{
    /// <summary>
    /// Defines interface to obtain a <see cref="MemoryDatabase"/> object.
    /// </summary>
    public interface IMemoryDatabaseProvider
    {
        /// <summary>
        /// Gets the <see cref="MemoryDatabase"/>.
        /// </summary>
        MemoryDatabase Database { get; }
    }
}