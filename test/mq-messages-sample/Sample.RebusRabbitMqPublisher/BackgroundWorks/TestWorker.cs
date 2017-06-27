using System;
using Abp.Dependency;
using Abp.MqMessages;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Sample.MqMessages;

namespace Sample.BackgroundWorks
{
    public class TestWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly IMqMessagePublisher _publisher;

        public TestWorker(AbpTimer timer, IMqMessagePublisher publisher)
            : base(timer)
        {
            _publisher = publisher;
            Timer.Period = 3000;//3 seconds
            Timer.RunOnStart = true;
        }

        protected override void DoWork()
        {
            Logger.Info($"TestWork Done! Time:{DateTime.Now}");
            _publisher.Publish(new TestMqMessage
            {
                Name = "TestWork",
                Value = "BlaBlaBlaBlaBlaBla",
                Time = DateTime.Now
            });
        }
    }
}
