using NHibernate;

namespace Adorable.NHibernate
{
    public interface ISessionProvider
    {
        ISession Session { get; }
    }
}