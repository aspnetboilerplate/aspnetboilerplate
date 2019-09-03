using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.NHibernate.Tests.Entities;
using Shouldly;
using Xunit;

namespace Abp.NHibernate.Tests
{
    public class Transaction_Tests : NHibernateTestBase
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public Transaction_Tests()
        {
            _bookRepository = Resolve<IRepository<Book>>();
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
        }

        [Fact]
        public void Should_Complete_Uow_Without_Transation()
        {
            var unitOfWorkOptions = new UnitOfWorkOptions
            {
                IsTransactional = false
            };

            using (var uow = _unitOfWorkManager.Begin(unitOfWorkOptions))
            {
                uow.Complete();
            }
        }

        [Fact]
        public async Task Should_Complete_Uow_Without_Transation_Async()
        {
            var unitOfWorkOptions = new UnitOfWorkOptions
            {
                IsTransactional = false
            };

            using (var uow = _unitOfWorkManager.Begin(unitOfWorkOptions))
            {
                await uow.CompleteAsync();
            }
        }

        [Fact]
        public void Should_Rollback_Transaction_On_Failure()
        {
            const string bookName = "Hitchhikers Guide to the Galaxy";

            try
            {
                using (var uow = _unitOfWorkManager.Begin())
                {
                    _bookRepository.Insert(new Book { Name = bookName });

                    throw new FakeException(); //Rollbacks transaction.
                }
            }
            catch (FakeException)
            {
                // Ignored
            }

            var insertedBook = UsingSession(session => session.Query<Book>().FirstOrDefault(b => b.Name == bookName));
            insertedBook.ShouldBeNull();
        }

        [Fact]
        public async Task Should_Rollback_Transaction_On_Failure_Async()
        {
            const string bookName = "Hitchhikers Guide to the Galaxy";

            try
            {
                using (var uow = _unitOfWorkManager.Begin())
                {
                    await _bookRepository.InsertAsync(new Book { Name = bookName });

                    throw new FakeException(); //Rollbacks transaction.
                }
            }
            catch (FakeException)
            {
                // Ignored
            }

            var insertedBook = UsingSession(session => session.Query<Book>().FirstOrDefault(b => b.Name == bookName));
            insertedBook.ShouldBeNull();
        }

        private class FakeException : Exception { }
    }
}
