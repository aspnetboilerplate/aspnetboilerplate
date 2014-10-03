using FluentNHibernate.Cfg;

namespace Abp.NHibernate.Config
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