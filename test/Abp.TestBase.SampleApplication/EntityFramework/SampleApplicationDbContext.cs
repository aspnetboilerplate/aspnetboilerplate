using System.Data.Common;
using System.Data.Entity;
using Abp.EntityFramework;
using Abp.TestBase.SampleApplication.ContacLists;
using Abp.TestBase.SampleApplication.Crm;
using Abp.TestBase.SampleApplication.GuidEntities;
using Abp.TestBase.SampleApplication.Messages;
using Abp.TestBase.SampleApplication.People;

namespace Abp.TestBase.SampleApplication.EntityFramework
{
    public class SampleApplicationDbContext : AbpDbContext
    {
        public virtual IDbSet<ContactList> ContactLists { get; set; }

        public virtual IDbSet<Person> People { get; set; }

        public virtual IDbSet<Message> Messages { get; set; }

        public virtual IDbSet<Company> Companies { get; set; }

        public virtual IDbSet<Branch> Branches { get; set; }

        public virtual IDbSet<TestEntityWithGuidPk> TestEntityWithGuidPks { get; set; }

        public virtual IDbSet<TestEntityWithGuidPkAndDbGeneratedValue> TestEntityWithGuidPkAndDbGeneratedValues { get; set; }

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
