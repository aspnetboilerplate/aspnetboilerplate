using System;
using System.Threading.Tasks;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Events.Bus;
using Shouldly;
using Xunit;

namespace Abp.Zero.BackgroundJobs
{
    public class BackgroundJobManagerEventTriggerExtensions_Tests : AbpZeroTestBase
    {
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IBackgroundJobStore _backgroundJobStore;

        public BackgroundJobManagerEventTriggerExtensions_Tests()
        {
            _backgroundJobManager = LocalIocManager.Resolve<IBackgroundJobManager>();
            _backgroundJobStore = LocalIocManager.Resolve<IBackgroundJobStore>();
        }

        [Fact]
        public async Task Should_Queue_Event_To_Background_Job()
        {
            for (int i = 0; i < 5; i++)
            {
                await _backgroundJobManager.EnqueueEventAsync(new MySimpleEventData(i));
            }

            var eventCount = await _backgroundJobStore.GetWaitingJobsAsync(5);
            eventCount.Count.ShouldBe(5);
        }

        [Fact]
        public async Task Queue_Event_Should_Return_Id_Of_Background_Job()
        {
            var id = await _backgroundJobManager.EnqueueAsync<TestJob, TestJobArgs>(new TestJobArgs());
            Convert.ToInt64(id).ShouldBeGreaterThan(0);
        }

        public class MySimpleEventData : EventData
        {
            public int Value { get; set; }

            public MySimpleEventData(int value)
            {
                Value = value;
            }
        }

        public class TestJob : BackgroundJob<TestJobArgs>, ITransientDependency
        {
            public override void Execute(TestJobArgs args)
            {

            }
        }

        [Serializable]
        public class TestJobArgs
        {

        }
    }
}
