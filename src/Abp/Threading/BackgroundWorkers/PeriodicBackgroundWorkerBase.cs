using Abp.Threading.Timers;

namespace Abp.Threading.BackgroundWorkers
{
    public abstract class PeriodicBackgroundWorkerBase : BackgroundWorkerBase
    {
        protected readonly AbpTimer Timer;

        protected PeriodicBackgroundWorkerBase(AbpTimer timer)
        {
            Timer = timer;
            Timer.Elapsed += Timer_Elapsed;
        }

        public override void Start()
        {
            base.Start();
            Timer.Start();
        }

        public override void Stop()
        {
            Timer.Stop();
            base.Stop();
        }

        public override void WaitToStop()
        {
            Timer.WaitToStop();
            base.WaitToStop();
        }
        
        protected abstract void Timer_Elapsed(object sender, System.EventArgs e);
    }
}