
/* see https://github.com/dotnet/corefx/issues/8681#issuecomment-290396746 */

//using System.IO;
//using System.Threading;
//using System.Threading.Tasks;
//using Shouldly;
//using Xunit;

//namespace Abp.TestBase.SampleApplication.Tests.Uow
//{
//    public class AsyncLocal_Tests
//    {
//        private static readonly AsyncLocal<string> _asyncLocal = new AsyncLocal<string>();

//        [Fact]
//        public async Task Test1()
//        {
//            _asyncLocal.Value = "XX";

//            await AsyncTestCode("42");

//            _asyncLocal.Value.ShouldBe("42");
//        }

//        private static async Task AsyncTestCode(string value)
//        {
//            using (var ms = new MemoryStream())
//            {
//                await ms.WriteAsync(new[] { (byte)1 }, 0, 1);

//                _asyncLocal.Value = value;
//                _asyncLocal.Value.ShouldBe(value);

//                await ms.WriteAsync(new[] { (byte)2 }, 0, 1);
//            }
//        }
//    }
//}
