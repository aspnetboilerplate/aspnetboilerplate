using NHibernate;
using Xunit;

namespace Abp.NHibernate.Tests
{
    public class SessionFactory_Test : NHibernateTestBase
    {
        public SessionFactory_Test()
        {
            _sessionFactory = Resolve<ISessionFactory>();
        }

        private readonly ISessionFactory _sessionFactory;

        [Fact]
        public void Should_OpenSession_Work()
        {
            using (var session = _sessionFactory.OpenSession())
            {
                //nothing...
            }
        }
    }
}