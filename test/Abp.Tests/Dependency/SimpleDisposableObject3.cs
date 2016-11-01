using System;

namespace Abp.Tests.Dependency
{
    public class SimpleDisposableObject3 : IDisposable
    {
        public int MyData { get; set; }

        public int DisposeCount { get; set; }

        public SimpleDisposableObject3()
        {

        }

        public SimpleDisposableObject3(int myData)
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