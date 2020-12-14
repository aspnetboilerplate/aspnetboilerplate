namespace Abp.Runtime.Caching.Redis
{
    /// <summary>
    /// An upper level container for <see cref="ICache"/> objects.
    /// <para>
    /// ICache objects requested from <see cref="IAbpPerRequestRedisCacheManager"/> stores redis caches per http context. And it does not try to pull it again from redis during the http context.
    /// It should not be used if the changes on this data in one instance of the project can causes an error in the other instance of the project.
    /// It is only recommended to use it in cases that you know cache will never be changed until current http context is done or it is not an important value. (for example some ui view settings etc.)
    /// Otherwise use <see cref="ICacheManager"/>
    /// </para>
    /// A cache manager should work as Singleton and track and manage <see cref="ICache"/> objects.
    /// </summary>
    public interface IAbpPerRequestRedisCacheManager : ICacheManager
    {
    }
}