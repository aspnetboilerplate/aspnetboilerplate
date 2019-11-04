using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.TestBase.SampleApplication.Crm;
using Abp.TestBase.SampleApplication.People;
using Abp.TestBase.SampleApplication.People.Dto;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.Auditing
{
    public class SimpleAuditing_Test : SampleApplicationTestBase
    {
        private readonly IPersonAppService _personAppService;
        private readonly AsyncCompanyAppService _asyncCompanyAppService;

        private IAuditingStore _auditingStore;

        public SimpleAuditing_Test()
        {
            _personAppService = Resolve<IPersonAppService>();
            _asyncCompanyAppService = Resolve<AsyncCompanyAppService>();
            Resolve<IAuditingConfiguration>().IsEnabledForAnonymousUsers = true;
            Resolve<IAuditingConfiguration>().SaveReturnValues = true;
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();
            _auditingStore = Substitute.For<IAuditingStore>();
            LocalIocManager.IocContainer.Register(
                Component.For<IAuditingStore>().Instance(_auditingStore).LifestyleSingleton()
                );
        }

        #region CASES WRITE AUDIT LOGS

        [Fact]

        public async Task Should_Write_Audits_For_Conventional_Methods()
        {
            /* All application service methods are audited as conventional. */

            await _personAppService.CreatePersonAsync(new CreatePersonInput { ContactListId = 1, Name = "john" });

#pragma warning disable 4014
            _auditingStore.Received().Save(Arg.Any<AuditInfo>());
#pragma warning restore 4014
        }

        [Fact]
        public void Should_Write_Audits_For_Audited_Class_Virtual_Methods_As_Default()
        {
            Resolve<MyServiceWithClassAudited>().Test1();
            _auditingStore.Received().Save(Arg.Is<AuditInfo>(a => a.ReturnValue == "1"));
        }

        [Fact]
        public async Task Should_Write_Audits_For_Audited_Class_Async_Methods_As_Default()
        {
            await Resolve<MyServiceWithClassAudited>().Test3().ConfigureAwait(false);
            _auditingStore.Received().Save(Arg.Is<AuditInfo>(a => a.ReturnValue == "1"));
        }

        [Fact]
        public async Task AuditInfo_ReturnValue_DisableAudit_Test()
        {
            Resolve<MyServiceWithClassAudited>().Test4();

            _auditingStore.Received().Save(Arg.Is<AuditInfo>(a =>
                !a.ReturnValue.Contains("123qwe")
            ));
        }

        [Fact]
        public void Should_Write_Audits_For_Audited_Methods()
        {
            Resolve<MyServiceWithMethodAudited>().Test1();
            _auditingStore.Received().Save(Arg.Any<AuditInfo>());
        }

        [Fact]
        public void Should_Write_Audits_For_AsyncCrudAppService_With_Correct_Service_Name()
        {
            _asyncCompanyAppService.DeleteAsync(new EntityDto(1));
            _auditingStore.Received().Save(Arg.Is<AuditInfo>(a => a.ServiceName.Contains("AsyncCompanyAppService")));
        }

        #endregion

        #region CASES DON'T WRITE AUDIT LOGS

        [Fact]
        public void Should_Not_Write_Audits_For_Conventional_Methods_If_Disabled_Auditing()
        {
            /* GetPeople has DisableAuditing attribute. */

            _personAppService.GetPeople(new GetPeopleInput());
            _auditingStore.DidNotReceive().SaveAsync(Arg.Any<AuditInfo>());
        }

        [Fact]
        public void Should_Not_Write_Audits_For_Audited_Class_Non_Virtual_Methods_As_Default()
        {
            Resolve<MyServiceWithClassAudited>().Test2();
            _auditingStore.DidNotReceive().SaveAsync(Arg.Any<AuditInfo>());
        }

        [Fact]
        public void Should_Not_Write_Audits_For_Not_Audited_Methods()
        {
            Resolve<MyServiceWithMethodAudited>().Test2();
            _auditingStore.DidNotReceive().SaveAsync(Arg.Any<AuditInfo>());
        }

        [Fact]
        public void Should_Not_Write_Audits_For_Not_Audited_Classes()
        {
            Resolve<MyServiceWithNotAudited>().Test1();
            _auditingStore.DidNotReceive().SaveAsync(Arg.Any<AuditInfo>());
        }


        [Fact]
        public void Should_Not_Write_Audits_If_Disabled()
        {
            Resolve<IAuditingConfiguration>().IsEnabled = false;
            Resolve<MyServiceWithMethodAudited>().Test1();
            _auditingStore.DidNotReceive().SaveAsync(Arg.Any<AuditInfo>());
        }

        #endregion

        [Audited]
        public class MyServiceWithClassAudited
        {
            public virtual int Test1()
            {
                return 1;
            }

            public void Test2()
            {

            }

            public virtual async Task<int> Test3()
            {
                var result = await Task.Factory.StartNew(() => 1).ConfigureAwait(false);
                await Task.Run(() => result + 1).ConfigureAwait(false);

                return result;
            }

            public virtual MyServiceWithClassAuditedTest4Output Test4()
            {
                return new MyServiceWithClassAuditedTest4Output
                {
                    Username = "admin",
                    Password = "123qwe"
                };
            }
        }

        public class MyServiceWithClassAuditedTest4Output
        {
            public string Username { get; set; }

            [DisableAuditing]
            public string Password { get; set; }
        }

        public class MyServiceWithMethodAudited
        {
            [Audited]
            public virtual void Test1()
            {

            }

            public virtual void Test2()
            {

            }
        }

        public class MyServiceWithNotAudited
        {
            public virtual void Test1()
            {

            }
        }
    }
}
