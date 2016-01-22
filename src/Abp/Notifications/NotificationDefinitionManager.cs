using System;
using System.Collections.Generic;
using Abp.Dependency;

namespace Abp.Notifications
{
    public class NotificationDefinitionManager : INotificationDefinitionManager, ISingletonDependency
    {
        private readonly INotificationConfiguration _configuration;
        private readonly IocManager _iocManager;

        public NotificationDefinitionManager(INotificationConfiguration configuration, IocManager iocManager)
        {
            _configuration = configuration;
            _iocManager = iocManager;
        }

        public void Initialize()
        {
            var context = new NotificationDefinitionContext(this);

            foreach (var providerType in _configuration.Providers)
            {
                var provider = CreateProvider(providerType);
                provider.SetNotifications(context);
            }
        }

        public void Add(NotificationDefinition notificationDefinition)
        {
            throw new System.NotImplementedException();
        }

        public NotificationDefinition Get(string name)
        {
            throw new System.NotImplementedException();
        }

        public NotificationDefinition GetOrNull(string name)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyList<NotificationDefinition> GetAll()
        {
            throw new System.NotImplementedException();
        }

        private NotificationProvider CreateProvider(Type providerType)
        {
            if (!_iocManager.IsRegistered(providerType))
            {
                _iocManager.Register(providerType);
            }

            return (NotificationProvider)_iocManager.Resolve(providerType);
        }
    }
}