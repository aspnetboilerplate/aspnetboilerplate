using Abp.Dependency;

namespace Abp.Domain.Repositories
{
    /// <summary>
    /// This interface must be implemented by all repositories to identify them by convention.
    /// Implement generic version instead of this one.
    /// </summary>
    public interface IRepository : ITransientDependency
    {
        //TODO: Can we remove this and use generic version to identify repositories?
    }
}