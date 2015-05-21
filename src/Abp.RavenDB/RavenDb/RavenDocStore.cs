using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abp.RavenDb.Configuration;
using Raven.Client;
using Raven.Client.Document;

namespace Abp.RavenDb
{
    static class RavenDocStore
    {
        static readonly ConcurrentDictionary<IAbpRavenDbModuleConfiguration, IDocumentStore> _docStoreMap = new ConcurrentDictionary<IAbpRavenDbModuleConfiguration, IDocumentStore>();

        public static IDocumentStore Store(IAbpRavenDbModuleConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            return _docStoreMap.GetOrAdd(configuration, CreateDocStore);
        }

        static IDocumentStore CreateDocStore(IAbpRavenDbModuleConfiguration configuration)
        {
            IDocumentStore store = new DocumentStore()
            {
                Url = configuration.Url,
                DefaultDatabase = configuration.DefaultDatatabaseName
            }.Initialize();

            return store;
        }
    }
}
