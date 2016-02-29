using FluentNHibernate.Cfg;

namespace Adorable.NHibernate.Configuration
{
    internal class AbpNHibernateModuleConfiguration : IAbpNHibernateModuleConfiguration
    {
        public FluentConfiguration FluentConfiguration { get; private set; }

        public AbpNHibernateModuleConfiguration()
        {
            FluentConfiguration = Fluently.Configure();
        }
    }
}