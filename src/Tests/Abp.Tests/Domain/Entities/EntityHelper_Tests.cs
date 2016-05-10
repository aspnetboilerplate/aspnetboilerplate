using System;
using Abp.Domain.Entities;
using Shouldly;
using Xunit;

namespace Abp.Tests.Domain.Entities
{
    public class EntityHelper_Tests
    {
        private class TestEntityWithGuidPk : Entity<Guid>
        {
        }

        [Fact]
        public void GetPrimaryKeyType_Tests()
        {
            EntityHelper.GetPrimaryKeyType<Manager>().ShouldBe(typeof(Guid));
            EntityHelper.GetPrimaryKeyType(typeof(Manager)).ShouldBe(typeof(Guid));
            EntityHelper.GetPrimaryKeyType(typeof(TestEntityWithGuidPk)).ShouldBe(typeof(Guid));
        }
    }
}