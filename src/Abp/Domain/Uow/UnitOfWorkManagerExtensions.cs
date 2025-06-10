using System;
using System.Threading.Tasks;

namespace Abp.Domain.Uow
{
    public static class UnitOfWorkManagerExtensions
    {
        public static void WithUnitOfWork(
            this IUnitOfWorkManager manager,
            Action action,
            UnitOfWorkOptions options = null)
        {
            using (var uow = manager.Begin(options ?? new UnitOfWorkOptions()))
            {
                action();
                uow.Complete();
            }
        }

        public static async Task WithUnitOfWorkAsync(
            this IUnitOfWorkManager manager,
            Func<Task> action,
            UnitOfWorkOptions options = null)
        {
            using (var uow = manager.Begin(options ?? new UnitOfWorkOptions()))
            {
                await action();
                await uow.CompleteAsync();
            }
        }

        public static TResult WithUnitOfWork<TResult>(
            this IUnitOfWorkManager manager,
            Func<TResult> action,
            UnitOfWorkOptions options = null)
        {
            TResult result;

            using (var uow = manager.Begin(options ?? new UnitOfWorkOptions()))
            {
                result = action();
                uow.Complete();
            }

            return result;
        }

        public static async Task<TResult> WithUnitOfWorkAsync<TResult>(
            this IUnitOfWorkManager manager,
            Func<Task<TResult>> action,
            UnitOfWorkOptions options = null)
        {
            TResult result;

            using (var uow = manager.Begin(options ?? new UnitOfWorkOptions()))
            {
                result = await action();
                await uow.CompleteAsync();
            }

            return result;
        }
    }
}
