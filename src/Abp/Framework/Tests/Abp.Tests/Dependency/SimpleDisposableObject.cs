using System;

namespace Abp.Tests.Dependency
{
    public class SimpleDisposableObject : IDisposable
    {
        public int DisposeCount { get; set; }

        public void Dispose()
        {
            DisposeCount++;
        }
    }
}