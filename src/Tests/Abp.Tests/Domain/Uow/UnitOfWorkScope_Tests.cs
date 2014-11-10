using Abp.Domain.Uow;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Xunit;

namespace Abp.Tests.Domain.Uow
{
    public class UnitOfWorkScope_Tests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void Should_Call_Uow_Methods()
        {
            var fakeUow = Substitute.For<IUnitOfWork>();

            LocalIocManager.IocContainer.Register(
                Component.For<IUnitOfWork>().UsingFactoryMethod(() => fakeUow).LifestyleTransient()
                );

            using (var unitOfWorkScope = new UnitOfWorkScope(LocalIocManager))
            {
                fakeUow.Received(1).Initialize(true);
                fakeUow.Received(1).Begin();

                unitOfWorkScope.Commit();
            }

            fakeUow.Received(1).End();
            fakeUow.Received(1).Dispose();
        }
    }
}
