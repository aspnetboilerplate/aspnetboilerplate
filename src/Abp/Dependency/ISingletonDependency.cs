namespace Abp.Dependency
{
    /// <summary>
    /// All classes implement this interface are automatically registered to dependency injection as singleton object.
    /// This means a single instance of the object will be kept during the entire lifetime of the application.
    /// </summary>
    public interface ISingletonDependency
    {
    }
}
