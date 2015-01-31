using Abp.Domain.Uow;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Xunit;

namespace Abp.Tests.Domain.Uow
{
    public class UnitOfWorkManager_Tests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void Should_Call_Uow_Methods()
        {
            var fakeUow = Substitute.For<IUnitOfWork>();
            
            LocalIocManager.IocContainer.Register(
                Component.For<IUnitOfWorkDefaultOptions>().ImplementedBy<UnitOfWorkDefaultOptions>().LifestyleSingleton(),
                Component.For<ICurrentUnitOfWorkProvider>().ImplementedBy<CallContextCurrentUnitOfWorkProvider>().LifestyleSingleton(),
                Component.For<IUnitOfWorkManager>().ImplementedBy<UnitOfWorkManager>().LifestyleSingleton(),
                Component.For<IUnitOfWork>().UsingFactoryMethod(() => fakeUow).LifestyleSingleton()
                );

            var uowManager = LocalIocManager.Resolve<IUnitOfWorkManager>();

            using (var uow1 = uowManager.Begin())
            {
                fakeUow.Received(1).Begin(Arg.Any<UnitOfWorkOptions>());

                using (var uow2 = uowManager.Begin())
                {
                    fakeUow.Received(1).Begin(Arg.Any<UnitOfWorkOptions>());

                    uow2.Complete();

                    fakeUow.DidNotReceive().Complete();
                }

                uow1.Complete();
            }

            fakeUow.Received(1).Complete();
            fakeUow.Received(1).Dispose();
        }
    }
}
