namespace Abp.Dependency
{
    /// <summary>
    /// All classes implement this interface are automatically registered to dependency injection as a transient object.
    /// This means each time the container is asked for a instance of this object, a new instance will be created.
    /// </summary>
    public interface ITransientDependency
    {
    }
}