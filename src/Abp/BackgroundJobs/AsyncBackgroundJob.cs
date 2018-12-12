using System;
using System.Threading.Tasks;
using Abp.Events.Bus;

namespace Abp.BackgroundJobs
{
    public abstract class AsyncBackgroundJob<T> : BackgroundJob<T>
    {
        public override void Execute(T args)
        {
            ExecuteAsync(args).Wait();
        }

        protected abstract Task ExecuteAsync(T args);
    }

}