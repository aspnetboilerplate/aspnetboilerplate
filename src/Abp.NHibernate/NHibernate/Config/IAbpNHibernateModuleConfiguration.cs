using FluentNHibernate.Cfg;

namespace Abp.NHibernate.Config
{
    /// <summary>
    /// Used to configure ABP NHibernate module.
    /// </summary>
    public interface IAbpNHibernateModuleConfiguration
    {
        /// <summary>
        /// Used to get and modify NHibernate fluent configuration.
        /// You can add mappings to this object.
        /// Do not call BuildSessionFactory on it.
        /// </summary>
        FluentConfiguration FluentConfiguration { get; }
    }
}