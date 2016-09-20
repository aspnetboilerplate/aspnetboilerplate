using System;
using System.Reflection;

namespace Abp.Aspects
{
    //THIS NAMESPACE IS WORK-IN-PROGRESS

    internal abstract class AspectAttribute : Attribute
    {
        public Type InterceptorType { get; set; }

        protected AspectAttribute(Type interceptorType)
        {
            InterceptorType = interceptorType;
        }
    }

    internal interface IAbpInterceptionContext
    {
        object Target { get; }

        MethodInfo Method { get; }

        object[] Arguments { get; }

        object ReturnValue { get; }

        bool Handled { get; set; }
    }

    internal interface IAbpBeforeExecutionInterceptionContext : IAbpInterceptionContext
    {

    }


    internal interface IAbpAfterExecutionInterceptionContext : IAbpInterceptionContext
    {
        Exception Exception { get; }
    }

    internal interface IAbpInterceptor<TAspect>
    {
        TAspect Aspect { get; set; }

        void BeforeExecution(IAbpBeforeExecutionInterceptionContext context);

        void AfterExecution(IAbpAfterExecutionInterceptionContext context);
    }

    internal abstract class AbpInterceptorBase<TAspect> : IAbpInterceptor<TAspect>
    {
        public TAspect Aspect { get; set; }

        public virtual void BeforeExecution(IAbpBeforeExecutionInterceptionContext context)
        {
        }

        public virtual void AfterExecution(IAbpAfterExecutionInterceptionContext context)
        {
        }
    }

    internal class Test_Aspects
    {
        internal class MyAspectAttribute : AspectAttribute
        {
            public int TestValue { get; set; }

            public MyAspectAttribute()
                : base(typeof(MyInterceptor))
            {
            }
        }

        internal class MyInterceptor : AbpInterceptorBase<MyAspectAttribute>
        {
            public override void BeforeExecution(IAbpBeforeExecutionInterceptionContext context)
            {
                Aspect.TestValue++;
            }

            public override void AfterExecution(IAbpAfterExecutionInterceptionContext context)
            {
                Aspect.TestValue++;
            }
        }

        public class MyService
        {
            [MyAspect(TestValue = 41)] //Usage!
            public void DoIt()
            {

            }
        }

        public class MyClient
        {
            private readonly MyService _service;

            public MyClient(MyService service)
            {
                _service = service;
            }

            public void Test()
            {
                _service.DoIt();
            }
        }
    }
}
