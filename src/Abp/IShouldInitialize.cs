using Castle.Core;

namespace Abp
{
    /// <summary>
    /// Defines interface for objects those should be Initialized before using it.
    /// If the object resolved using dependency injection, <see cref="IInitializable.Initialize"/>
    /// method is automatically called just after creation of the object.
    /// </summary>
    public interface IShouldInitialize : IInitializable
    {
        
    }
}