using System.Linq;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.TestBase.SampleApplication.People;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.Uow
{
    public class Custom_Uow_Convention_Tests : SampleApplicationTestBase
    {
        private readonly MyCustomUowClass _customUowClass;

        public Custom_Uow_Convention_Tests()
        {
            _customUowClass = Resolve<MyCustomUowClass>();
        }

        [Fact]
        public void Should_Apply_Custom_UnitOfWork_Convention()
        {
            _customUowClass.GetPeopleCount().ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Should_Not_Intercept_Non_Uow_Marked_Methods()
        {
            _customUowClass.NonUowMethod();
        }
    }

    public class MyCustomUowClass : ITransientDependency
    {
        private readonly IRepository<Person> _personRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public MyCustomUowClass(IRepository<Person> personRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _personRepository = personRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public virtual int GetPeopleCount()
        {
            //GetAll can be only used inside a UOW. This should work since MyCustomUowClass is UOW by custom convention.
            _unitOfWorkManager.Current.ShouldNotBeNull();
            return _personRepository.GetAll().Count();
        }

        [UnitOfWork(IsDisabled = true)]
        public virtual void NonUowMethod()
        {
            _unitOfWorkManager.Current.ShouldBeNull();
        }
    }
}
