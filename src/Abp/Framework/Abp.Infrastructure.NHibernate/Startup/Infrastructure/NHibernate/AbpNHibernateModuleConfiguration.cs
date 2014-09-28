using FluentNHibernate.Cfg;

namespace Abp.Startup.Infrastructure.NHibernate
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