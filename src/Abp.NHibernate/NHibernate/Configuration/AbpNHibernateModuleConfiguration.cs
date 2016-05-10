using FluentNHibernate.Cfg;

namespace Abp.NHibernate.Configuration
{
    internal class AbpNHibernateModuleConfiguration : IAbpNHibernateModuleConfiguration
    {
        public AbpNHibernateModuleConfiguration()
        {
            FluentConfiguration = Fluently.Configure();
        }

        public FluentConfiguration FluentConfiguration { get; }
    }
}