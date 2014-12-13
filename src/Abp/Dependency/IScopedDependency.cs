namespace Abp.Dependency
{
    /// <summary>
    /// All classes implement this interface are automatically registered to dependency injection a scoped object.
    /// This means during the execution of a scope (which can be a web request or a thread), the same object 
    /// instance will be returned, regardless of how many times the container is asked for this object.
    /// </summary>
    public interface IScopedDependency
    {
    }
}
