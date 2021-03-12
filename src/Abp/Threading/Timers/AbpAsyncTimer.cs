using System;
using System.Threading;
using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.Threading.Timers
{
    public class AbpAsyncTimer : RunnableBase, ITransientDependency
    {
        /// <summary>
        /// This event is raised periodically according to Period of Timer.
        /// </summary>
        public Func<AbpAsyncTimer, Task> Elapsed = _ => Task.CompletedTask;

        /// <summary>
        /// Task period of timer (as milliseconds).
        /// </summary>
        public int Period { get; set; }

        /// <summary>
        /// Indicates whether timer raises Elapsed event on Start method of Timer for once.
        /// Default: False.
        /// </summary>
        public bool RunOnStart { get; set; }

        /// <summary>
        /// This timer is used to perfom the task at spesified intervals.
        /// </summary>
        private readonly Timer _taskTimer;

        /// <summary>
        /// Indicates that whether timer is running or stopped.
        /// </summary>
        private volatile bool _running;

        /// <summary>
        /// Indicates that whether performing the task or _taskTimer is in sleep mode.
        /// This field is used to wait executing tasks when stopping Timer.
        /// </summary>
        private volatile bool _performingTasks;

        /// <summary>
        /// Creates a new Timer.
        /// </summary>
        public AbpAsyncTimer()
        {
            _taskTimer = new Timer(TimerCallBack, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Creates a new Timer.
        /// </summary>
        /// <param name="period">Task period of timer (as milliseconds)</param>
        /// <param name="runOnStart">Indicates whether timer raises Elapsed event on Start method of Timer for once</param>
        public AbpAsyncTimer(int period, bool runOnStart = false)
            : this()
        {
            Period = period;
            RunOnStart = runOnStart;
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        public override void Start()
        {
            if (Period <= 0)
            {
                throw new AbpException("Period should be set before starting the timer!");
            }

            base.Start();

            _running = true;
            _taskTimer.Change(RunOnStart ? 0 : Period, Timeout.Infinite);
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public override void Stop()
        {
            lock (_taskTimer)
            {
                _running = false;
                _taskTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }

            base.Stop();
        }

        /// <summary>
        /// Waits the service to stop.
        /// </summary>
        public override void WaitToStop()
        {
            lock (_taskTimer)
            {
                while (_performingTasks)
                {
                    Monitor.Wait(_taskTimer);
                }
            }

            base.WaitToStop();
        }

        /// <summary>
        /// This method is called by _taskTimer.
        /// </summary>
        /// <param name="state">Not used argument</param>
        private void TimerCallBack(object state)
        {
            lock (_taskTimer)
            {
                if (!_running || _performingTasks)
                {
                    return;
                }

                _taskTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _performingTasks = true;
            }

            _ = Timer_Elapsed();
        }
        
        private async Task Timer_Elapsed()
        {
            try
            {
                await Elapsed(this);
            }
            catch(Exception ex)
            {
                
            }
            finally
            {
                lock (_taskTimer)
                {
                    _performingTasks = false;
                    if (IsRunning)
                    {
                        _taskTimer.Change(Period, Timeout.Infinite);
                    }

                    Monitor.Pulse(_taskTimer);
                }
            }
        }
    }
}
