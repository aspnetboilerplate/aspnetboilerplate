using System;

namespace Abp.Utils.Etc
{
    /// <summary>
    /// This class is used to simulate a Disposable that does nothing.
    /// </summary>
    internal class NullDisposable : IDisposable
    {
        public void Dispose()
        {
            
        }
    }
}