using NHibernate;

namespace Abp.NHibernate
{
    public interface ISessionProvider
    {
        ISession Session { get; }
    }
}