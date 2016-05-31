using System;
using System.Linq;
using System.Reflection;
using Abp.Application.Services;
using Abp.Dependency;
using Abp.WebApi.Controllers.Dynamic;
using Abp.WebApi.Controllers.Dynamic.Builders;
using Shouldly;
using Xunit;

namespace Abp.Web.Api.Tests.DynamicApiController.BatchBuilding
{
    public class BatchDynamicApiControllerBuilder_Test
    {
        [Fact]
        public void Test1()
        {
            IocManager.Instance.Register<IMyFirstAppService, MyFirstAppService>();

            DynamicApiControllerBuilder
                .ForAll<IApplicationService>(Assembly.GetExecutingAssembly(), "myapp")
                .Where(type => type == typeof(IMyFirstAppService))
                .ForMethods(builder =>
                {
                    if (builder.Method.IsDefined(typeof(MyIgnoreApiAttribute)))
                    {
                        builder.DontCreate = true;
                    }
                })
                .Build();

            var services = DynamicApiControllerManager.GetAll();
            services.Count.ShouldBe(1);
            services[0].ServiceName.ShouldBe("myapp/myFirst");
            services[0].Actions.Count.ShouldBe(1);
            services[0].Actions.ContainsKey("GetStr").ShouldBe(true);
        }

        public interface IMyFirstAppService : IApplicationService
        {
            string GetStr(int i);

            [MyIgnoreApi]
            string GetStr2(int i);
        }

        public class MyFirstAppService : IMyFirstAppService
        {
            public string GetStr(int i)
            {
                return i.ToString();
            }

            public string GetStr2(int i)
            {
                return (i + 1).ToString();
            }
        }

        public class MyIgnoreApiAttribute : Attribute
        {

        }
    }
}