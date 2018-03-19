using System;
using System.Threading;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Quartz.Configuration;
using Abp.TestBase;
using Quartz;
using Shouldly;
using Xunit;

namespace Abp.Quartz.Tests
{
    public class AbpQuartzConfigurationTests
    {
        [Fact]
        public void TestGetScheduler()
        {
            var config = new AbpQuartzConfiguration();
            var t1 = Task.Run(() => config.Scheduler);
            var t2 = Task.Run(() => config.Scheduler);
            Task.WaitAll(t1, t2);
            Assert.True(true);
        }
    }
}
