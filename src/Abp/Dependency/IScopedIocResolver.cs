using System;

namespace Abp.Dependency
{
    /// <summary>
    ///     This interface is used to wrap a scope for batch resolvings in a single <c>using</c> statement.
    ///     It inherits <see cref="IDisposable" /> and <see cref="IIocResolver" />, so resolved objects can be easily and batch
    ///     manner released by IocResolver.
    ///     In <see cref="IDisposable.Dispose" /> method, <see cref="IIocResolver.Release" /> is called to dispose the object.
    /// </summary>
    public interface IScopedIocResolver : IIocResolver, IDisposable { }
}