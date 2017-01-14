using Abp.Auditing;
using Shouldly;
using Xunit;

namespace Abp.Tests.Auditing
{
    public class AuditingHelper_Tests
    {
        [Fact]
        public void Ignored_Properties_Should_Not_Be_Serialized()
        {
            var json = AuditingHelper.Serialize(new AuditingHelperTestPersonDto
            {
                FullName = "John Doe",
                Age = 18,
                School = new AuditingHelperTestSchoolDto
                {
                    Name = "Crosswell Secondary",
                    Address = "Broadway Ave, West Bend"
                }
            });

            json.ShouldBe("{\"fullName\":\"John Doe\",\"school\":{\"name\":\"Crosswell Secondary\"}}");
        }

        public class AuditingHelperTestPersonDto
        {
            public string FullName { get; set; }

            [DisableAuditing]
            public int Age { get; set; }

            public AuditingHelperTestSchoolDto School { get; set; }
        }

        public class AuditingHelperTestSchoolDto
        {
            public string Name { get; set; }

            [DisableAuditing]
            public string Address { get; set; }
        }
    }
}
