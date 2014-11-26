using Abp.Dependency;

namespace Abp.Domain.Repositories
{
    /// <summary>
    /// This interface must be implemented by all async repositories to identify them by convention.
    /// Implement generic version instead of this one.
    /// </summary>
    public interface IAsyncRepository : ITransientDependency
    {
    }
}
