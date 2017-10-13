using System;
using System.Reflection;
using Abp.Events.Bus.Handlers;
using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Shouldly;
using Xunit;

namespace Abp.Tests.Castle
{
    public class Castle_Interception_Test
    {
        private WindsorContainer _container;
        private MyHandler _handler;

        public Castle_Interception_Test()
        {
            _container = new WindsorContainer();

            _container.Register(
                Component.For<MyInterceptor>().LifestyleTransient(),
                Component.For<MyHandler>().Interceptors<MyInterceptor>().LifestyleTransient()
            );

            _handler = _container.Resolve<MyHandler>();
        }

        [Fact]
        public void Test_Regular()
        {
            _handler.HandleEvent(new MyEventData());
        }

        [Fact]
        public void Test_Reflection()
        {
            typeof(IEventHandler<MyEventData>)
                .GetMethod("HandleEvent", BindingFlags.Instance | BindingFlags.Public)
                .Invoke(_handler, new object[] {new MyEventData()});
        }

        public class MyHandler : IEventHandler<MyEventData>
        {
            public bool IsIntercepted { get; set; }

            public virtual void HandleEvent(MyEventData eventData)
            {
                IsIntercepted.ShouldBeTrue();
            }
        }

        public class MyEventData
        {
            
        }

        public class MyInterceptor : IInterceptor
        {
            public void Intercept(IInvocation invocation)
            {
                (invocation.InvocationTarget as MyHandler).IsIntercepted = true;
            }
        }
    }
}
