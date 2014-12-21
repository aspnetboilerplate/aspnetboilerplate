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
                Component.For<ICurrentUnitOfWorkProvider>().ImplementedBy<ThreadStaticCurrentUnitOfWorkProvider>().LifestyleSingleton(),
                Component.For<IUnitOfWorkManager>().ImplementedBy<UnitOfWorkManager>().LifestyleSingleton(),
                Component.For<IUnitOfWork>().UsingFactoryMethod(() => fakeUow).LifestyleTransient()
                );

            var uowManager = LocalIocManager.Resolve<IUnitOfWorkManager>();

            using (var uow1 = uowManager.StartNew())
            {
                fakeUow.Received(1).Start();

                using (var uow2 = uowManager.StartNew())
                {
                    fakeUow.Received(1).Start();

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
