using System;

namespace Abp.Tests.Dependency
{
    public class SimpleDisposableObject : IDisposable
    {
        public int MyData { get; set; }

        public int DisposeCount { get; set; }

        public SimpleDisposableObject()
        {
            
        }

        public SimpleDisposableObject(int myData)
        {
            MyData = myData;
        }

        public void Dispose()
        {
            DisposeCount++;
        }

        public int GetMyData()
        {
            return MyData;
        }
    }
}