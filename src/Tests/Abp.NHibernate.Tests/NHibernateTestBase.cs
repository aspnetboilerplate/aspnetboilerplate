using System;
using Abp.Collections;
using Abp.Modules;
using Abp.TestBase;
using NHibernate;

namespace Abp.NHibernate.Tests
{
    public class NHibernateTestBase : AbpIntegratedTestBase
    {
        protected override void AddModules(ITypeList<AbpModule> modules)
        {
            base.AddModules(modules);
            modules.Add<NHibernateTestModule>();
        }

        public void UsingSession(Action<ISession> action)
        {
            using (var session = LocalIocManager.Resolve<ISessionFactory>().OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    action(session);
                    transaction.Commit();
                }
            }
        }

        public T UsingSession<T>(Func<ISession, T> func)
        {
            T result;

            using (var session = LocalIocManager.Resolve<ISessionFactory>().OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    result = func(session);
                    transaction.Commit();
                }
            }

            return result;
        }
    }
}