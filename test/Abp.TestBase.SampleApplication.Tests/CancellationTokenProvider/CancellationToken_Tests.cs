using Abp.Threading;
using System;
using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Threading;
using Shouldly;

namespace Abp.TestBase.SampleApplication.Tests.CancellationTokenProvider
{
    public class CancellationToken_Tests : SampleApplicationTestBase
    {
        public ICancellationTokenProvider CancellationTokenProvider;

        protected override void PreInitialize()
        {
            base.PreInitialize();
            LocalIocManager.IocContainer.Register(
                Component.For<ICancellationTokenProvider>().Instance(NullCancellationTokenProvider.Instance)
            );
        }

        public CancellationToken_Tests()
        {
            CancellationTokenProvider = LocalIocManager.Resolve<ICancellationTokenProvider>();
        }

        [Fact]
        public async Task Should_Cancel_After_1_Second()
        {
            // Signal cancellation after 5 seconds
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(1));
            bool isCanceled = false;
            using (CancellationTokenProvider.Use(cts.Token))
            {
                try
                {
                    var result = await LongTask(1000, cts.Token);
                }
                catch (Exception)
                {
                    isCanceled = true;
                }
            }
            isCanceled.ShouldBeTrue();
        }

        [Fact]
        public async Task Should_Not_Cancel_Operation()
        {
            bool isCanceled = false;
            using (CancellationTokenProvider.Use(CancellationToken.None))
            {
                try
                {
                    await LongTask(100);
                }
                catch (Exception)
                {
                    isCanceled = true;
                }
            }
            isCanceled.ShouldBeFalse();
        }

        public Task<decimal> LongTask(int loopCounter, CancellationToken cancellationToken = default)
        {
            Task<decimal> task = null;

            // Start a task a return it
            task = Task.Run(() =>
            {
                decimal result = 0;

                // Loop for a defined number of iterations
                for (int i = 0; i < loopCounter; i++)
                {
                    // Check if a cancellation is requested, if yes,
                    // throw a TaskCanceledException.
                    if (cancellationToken.IsCancellationRequested)
                        throw new TaskCanceledException(task);

                    cancellationToken.ThrowIfCancellationRequested();

                    Thread.Sleep(10);
                    result += i;
                }
                return result;
            });
            return task;
        }
    }
}
