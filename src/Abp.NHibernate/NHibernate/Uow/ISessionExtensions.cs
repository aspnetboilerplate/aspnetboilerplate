using NHibernate;

namespace Abp.NHibernate.Uow
{
    public static class NhSessionExtensions
    {
        public static bool IsFilterEnabled(this ISession session, string filterName)
        {
            var filter = session.GetEnabledFilter(filterName);
            return filter != null;
        }
    }
}
