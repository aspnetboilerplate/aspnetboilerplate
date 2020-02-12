using Shouldly;
using Xunit;

namespace Abp.Tests
{
    public class UserIdentifier_Tests
    {
        [Fact]
        public void GetHashCode_Test()
        {
            UserIdentifier.Parse("5@4").GetHashCode().ShouldBe(UserIdentifier.Parse("5@4").GetHashCode());

            UserIdentifier.Parse("1@1").GetHashCode().ShouldNotBe(UserIdentifier.Parse("2@2").GetHashCode());

            UserIdentifier.Parse("1@0").GetHashCode().ShouldNotBe(UserIdentifier.Parse("0@1").GetHashCode());

            UserIdentifier.Parse("1@2").GetHashCode().ShouldNotBe(UserIdentifier.Parse("2@1").GetHashCode());
        }
    }
}