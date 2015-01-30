using System.Data.Common;
using System.Data.Entity;
using Abp.EntityFramework;
using Abp.TestBase.SampleApplication.People;

namespace Abp.TestBase.SampleApplication.EntityFramework
{
    public class SampleApplicationDbContext : AbpDbContext
    {
        public virtual IDbSet<Person> People { get; set; }

        public SampleApplicationDbContext()
        {

        }

        public SampleApplicationDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        public SampleApplicationDbContext(DbConnection connection)
            : base(connection, true)
        {

        }
    }
}
