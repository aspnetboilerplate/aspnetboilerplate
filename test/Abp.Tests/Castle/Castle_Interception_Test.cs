using System;
using System.Reflection;
using System.Threading.Tasks;
using Abp.Events.Bus.Handlers;
using Abp.Threading;
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
        private MyAsyncHandler _asyncHandler;

        public Castle_Interception_Test()
        {
            _container = new WindsorContainer();

            _container.Register(
                Component.For<MyInterceptor>().LifestyleTransient(),
                Component.For<MyHandler>().Interceptors<MyInterceptor>().LifestyleTransient(),
                Component.For<MyAsyncInterceptor>().LifestyleTransient(),
                Component.For<MyAsyncHandler>().Interceptors<MyAsyncInterceptor>().LifestyleTransient()
            );

            _handler = _container.Resolve<MyHandler>();
            _asyncHandler = _container.Resolve<MyAsyncHandler>();
        }

        [Fact]
        public void Test_Regular()
        {
            _handler.HandleEvent(new MyEventData());
            AsyncHelper.RunSync(() => _asyncHandler.HandleEventAsync(new MyEventData()));
        }

        [Fact]
        public void Test_Reflection()
        {
            typeof(IEventHandler<MyEventData>)
                .GetMethod("HandleEvent", BindingFlags.Instance | BindingFlags.Public)
                .Invoke(_handler, new object[] {new MyEventData()});

            AsyncHelper.RunSync(
                () => 
                {
                    return (Task) typeof(IAsyncEventHandler<MyEventData>)
                        .GetMethod("HandleEventAsync", BindingFlags.Instance | BindingFlags.Public)
                        .Invoke(_asyncHandler, new object[] { new MyEventData() });
                });
        }

        public class MyHandler : IEventHandler<MyEventData>
        {
            public bool IsIntercepted { get; set; }

            public virtual void HandleEvent(MyEventData eventData)
            {
                IsIntercepted.ShouldBeTrue();
            }
        }

        public class MyAsyncHandler : IAsyncEventHandler<MyEventData>
        {
            public bool IsIntercepted { get; set; }

            public virtual Task HandleEventAsync(MyEventData eventData)
            {
                IsIntercepted.ShouldBeTrue();
                return Task.CompletedTask;
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
                invocation.Proceed();
            }
        }

        public class MyAsyncInterceptor : IInterceptor
        {
            public void Intercept(IInvocation invocation)
            {
                (invocation.InvocationTarget as MyAsyncHandler).IsIntercepted = true;
                invocation.Proceed();
            }
        }
    }
}
