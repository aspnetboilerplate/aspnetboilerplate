using NHibernate;

namespace Abp.Zero.SampleApp.NHibernate.TestDatas
{
    public class InitialTestDataBuilder
    {
        private readonly ISession _session;

        public InitialTestDataBuilder(ISession session)
        {
            _session = session;
        }

        public void Build()
        {
            new InitialTenantsBuilder(_session).Build();
            new InitialUsersBuilder(_session).Build();
            new InitialTestLanguagesBuilder(_session).Build();
            new InitialTestOrganizationUnitsBuilder(_session).Build();
            new InitialUserOrganizationUnitsBuilder(_session).Build();
        }
    }
}