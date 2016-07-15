using FluentNHibernate.Cfg;

namespace Abp.NHibernate.Configuration
{
    internal class AbpNHibernateModuleConfiguration : IAbpNHibernateModuleConfiguration
    {
        public FluentConfiguration FluentConfiguration { get; }

        public AbpNHibernateModuleConfiguration()
        {
            FluentConfiguration = Fluently.Configure();
        }
    }
}