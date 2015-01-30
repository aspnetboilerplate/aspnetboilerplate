using System;
using System.Threading.Tasks;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.TestUtils
{
    public static class AssertEx
    {
        //Copied from: http://stackoverflow.com/questions/14084923/how-to-handle-exceptions-thrown-by-tasks-in-xunit-nets-assert-throwst
        public static async Task ThrowsAsync<TException>(Func<Task> func)
        {
            var expected = typeof(TException);
            Type actual = null;
            try
            {
                await func();
            }
            catch (Exception e)
            {
                actual = e.GetType();
            }
            Assert.Equal(expected, actual);
        }
    }
}