using System.Linq;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.TestBase.SampleApplication.People;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.Uow
{
    public class UnitOfWork_Class_Attribute_Test : SampleApplicationTestBase
    {
        private readonly MyUowMarkedClass _uowClass;
        private readonly MyDisabledUowMarkedClass _disabledUowClass;

        public UnitOfWork_Class_Attribute_Test()
        {
            _uowClass = Resolve<MyUowMarkedClass>();
            _disabledUowClass = Resolve<MyDisabledUowMarkedClass>();
        }

        [Fact]
        public void Should_Intercept_Uow_Marked_Classes()
        {
            _uowClass.GetPeopleCount().ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Should_Not_Intercept_Non_Uow_Marked_Methods()
        {
            _uowClass.NonUowMethod();
        }

        [Fact]
        public void Should_Intercept_Uow_Marked_Methods_On_Disabled_Uow_Classes()
        {
            _disabledUowClass.GetPeopleCount().ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Should_Not_Intercept_Non_Uow_Marked_Classes()
        {
            _disabledUowClass.NonUowMethod();
        }
    }

    [UnitOfWork]
    public class MyUowMarkedClass : ITransientDependency
    {
        private readonly IRepository<Person> _personRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public MyUowMarkedClass(IRepository<Person> personRepository, IUnitOfWorkManager unitOfWorkManager)
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

    [UnitOfWork(IsDisabled = true)]
    public class MyDisabledUowMarkedClass : ITransientDependency
    {
        private readonly IRepository<Person> _personRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public MyDisabledUowMarkedClass(IRepository<Person> personRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _personRepository = personRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        [UnitOfWork(IsDisabled = false)]
        public virtual int GetPeopleCount()
        {
            //GetAll can be only used inside a UOW. This should work since MyCustomUowClass is UOW by custom convention.
            _unitOfWorkManager.Current.ShouldNotBeNull();
            return _personRepository.GetAll().Count();
        }

        public virtual void NonUowMethod()
        {
            _unitOfWorkManager.Current.ShouldBeNull();
        }
    }
}
