using System;
using Abp.Domain.Uow;
using Shouldly;
using Xunit;

namespace Abp.Tests.Domain.Uow
{
    public class InnerUnitOfWorkCompleteHandle_Test
    {
        [Fact]
        public void Should_Not_Throw_Exception_If_Complete_Called()
        {
            using (var uow = new InnerUnitOfWorkCompleteHandle())
            {
                uow.Complete();
            }
        }

        [Fact]
        public void Should_Throw_Exception_If_Complete_Did_Not_Called()
        {
            Assert.Throws<AbpException>(() =>
            {
                using (var uow = new InnerUnitOfWorkCompleteHandle())
                {

                }
            }).Message.ShouldBe(InnerUnitOfWorkCompleteHandle.DidNotCallCompleteMethodExceptionMessage);
        }

        [Fact]
        public void Should_Not_Override_Exception_If_Exception_Is_Thrown_By_User()
        {
            Assert.Throws<Exception>(
                new Action(() =>
                           {
                               using (var uow = new InnerUnitOfWorkCompleteHandle())
                               {
                                   throw new Exception("My inner exception!");
                               }
                           })).Message.ShouldBe("My inner exception!");
        }
    }
}