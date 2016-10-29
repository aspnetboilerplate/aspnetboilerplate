using System;

namespace Abp.Tests.Dependency
{
    public class SimpleDisposableObject2 : IDisposable
    {
        public int MyData { get; set; }

        public int DisposeCount { get; set; }

        public SimpleDisposableObject2()
        {

        }

        public SimpleDisposableObject2(int myData)
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