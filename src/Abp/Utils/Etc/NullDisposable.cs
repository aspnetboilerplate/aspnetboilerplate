using System;

namespace Abp.Utils.Etc
{
    /// <summary>
    ///     This class is used to simulate a Disposable that does nothing.
    /// </summary>
    internal sealed class NullDisposable : IDisposable
    {
        private NullDisposable()
        {
        }

        public static NullDisposable Instance { get; } = new NullDisposable();

        public void Dispose()
        {
        }
    }
}