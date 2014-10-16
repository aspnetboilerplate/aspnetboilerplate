using Abp.Dependency;
using Castle.MicroKernel;
using Shouldly;
using Xunit;

namespace Abp.Tests.Dependency
{
    public class CircularDependency_Tests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void Should_Fail_Circular_Constructor_Dependency()
        {
            LocalIocManager.Register<MyClass1>();
            LocalIocManager.Register<MyClass2>();
            LocalIocManager.Register<MyClass3>();

            Assert.Throws<CircularDependencyException>(() => LocalIocManager.Resolve<MyClass1>());
        }

        [Fact]
        public void Should_Success_Circular_Property_Injection_Transient()
        {
            MyClass4.CreateCount = 0;
            MyClass5.CreateCount = 0;
            MyClass6.CreateCount = 0;

            LocalIocManager.Register<MyClass4>(DependencyLifeStyle.Transient);
            LocalIocManager.Register<MyClass5>(DependencyLifeStyle.Transient);
            LocalIocManager.Register<MyClass6>(DependencyLifeStyle.Transient);

            var obj4 = LocalIocManager.Resolve<MyClass4>();
            obj4.Obj5.ShouldNotBe(null);
            obj4.Obj6.ShouldNotBe(null);
            obj4.Obj5.Obj6.ShouldNotBe(null);

            var obj5 = LocalIocManager.Resolve<MyClass5>();
            obj5.Obj4.ShouldNotBe(null);
            obj5.Obj6.ShouldNotBe(null);
            obj5.Obj4.Obj6.ShouldNotBe(null);

            MyClass4.CreateCount.ShouldBe(2);
            MyClass5.CreateCount.ShouldBe(2);
            MyClass6.CreateCount.ShouldBe(4);
        }

        [Fact]
        public void Should_Success_Circular_Property_Injection_Singleton()
        {
            MyClass4.CreateCount = 0;
            MyClass5.CreateCount = 0;
            MyClass6.CreateCount = 0;

            LocalIocManager.Register<MyClass4>(DependencyLifeStyle.Singleton);
            LocalIocManager.Register<MyClass5>(DependencyLifeStyle.Singleton);
            LocalIocManager.Register<MyClass6>(DependencyLifeStyle.Singleton);

            var obj4 = LocalIocManager.Resolve<MyClass4>();
            obj4.Obj5.ShouldNotBe(null);
            obj4.Obj6.ShouldNotBe(null);
            obj4.Obj5.Obj6.ShouldNotBe(null);

            var obj5 = LocalIocManager.Resolve<MyClass5>();
            obj5.Obj4.ShouldBe(null); //!!!Notice: It's null
            obj5.Obj6.ShouldNotBe(null);

            MyClass4.CreateCount.ShouldBe(1);
            MyClass5.CreateCount.ShouldBe(1);
            MyClass6.CreateCount.ShouldBe(1);
        }


        #region Failing depended classes

        public class MyClass1
        {
            public MyClass1(MyClass2 obj)
            {

            }
        }

        public class MyClass2
        {
            public MyClass2(MyClass3 obj)
            {

            }
        }

        public class MyClass3
        {
            public MyClass3(MyClass1 obj)
            {

            }
        }

        #endregion

        public class MyClass4
        {
            public static int CreateCount { get; set; }

            public MyClass5 Obj5 { get; set; }

            public MyClass6 Obj6 { get; set; }

            public MyClass4()
            {
                CreateCount++;
            }
        }

        public class MyClass5
        {
            public static int CreateCount { get; set; }
            public MyClass4 Obj4 { get; set; }

            public MyClass6 Obj6 { get; set; }

            public MyClass5()
            {
                CreateCount++;
            }
        }

        public class MyClass6
        {
            public static int CreateCount { get; set; }

            public MyClass6()
            {
                CreateCount++;
            }
        }
    }
}