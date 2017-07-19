using System;
using Abp.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace AbpAspNetCoreDemo.Controllers
{
    public class LifestyleDemoController : AbpController
    {
        private readonly MyTransientClass1 _myTransientClass1;
        private readonly MyTransientClass2 _myTransientClass2;

        public LifestyleDemoController(MyTransientClass1 myTransientClass1, MyTransientClass2 myTransientClass2)
        {
            _myTransientClass1 = myTransientClass1;
            _myTransientClass2 = myTransientClass2;
        }

        public ActionResult Test1()
        {
            return new ContentResult()
            {
                ContentType = "text/plain",
                Content = $"_myTransientClass1.ScopedClass.Id = {_myTransientClass1.ScopedClass.Id}; _myTransientClass2.ScopedClass.Id = {_myTransientClass2.ScopedClass.Id}"
            };
        }
    }

    public class MyTransientClass1
    {
        public MyScopedClass ScopedClass { get; }

        public MyTransientClass1(MyScopedClass scopedClass)
        {
            ScopedClass = scopedClass;
        }
    }

    public class MyTransientClass2
    {
        public MyScopedClass ScopedClass { get; }

        public MyTransientClass2(MyScopedClass scopedClass)
        {
            ScopedClass = scopedClass;
        }
    }

    public class MyScopedClass
    {
        public string Id { get; } = Guid.NewGuid().ToString("N");
    }
}
