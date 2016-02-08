using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Abp.Application.Features;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Dependency;

namespace Abp.Notifications
{
    /// <summary>
    /// Implements <see cref="INotificationDefinitionManager"/>.
    /// </summary>
    internal class NotificationDefinitionManager : INotificationDefinitionManager, ISingletonDependency
    {
        private readonly INotificationConfiguration _configuration;
        private readonly IPermissionDependencyContext _permissionDependencyContext;
        private readonly IFeatureDependencyContext _featureDependencyContext;
        private readonly IocManager _iocManager;

        private readonly IDictionary<string, NotificationDefinition> _notificationDefinitions;

        public NotificationDefinitionManager(
            IocManager iocManager,
            INotificationConfiguration configuration,
            IPermissionDependencyContext permissionDependencyContext,
            IFeatureDependencyContext featureDependencyContext)
        {
            _configuration = configuration;
            _permissionDependencyContext = permissionDependencyContext;
            _featureDependencyContext = featureDependencyContext;
            _iocManager = iocManager;

            _notificationDefinitions = new Dictionary<string, NotificationDefinition>();
        }

        public void Initialize()
        {
            var context = new NotificationDefinitionContext(this);

            foreach (var providerType in _configuration.Providers)
            {
                _iocManager.RegisterIfNot(providerType, DependencyLifeStyle.Transient);
                using (var provider = _iocManager.ResolveAsDisposable<NotificationProvider>(providerType))
                {
                    provider.Object.SetNotifications(context);
                }
            }
        }

        public void Add(NotificationDefinition notificationDefinition)
        {
            if (_notificationDefinitions.ContainsKey(notificationDefinition.Name))
            {
                throw new AbpInitializationException("There is already a notification definition with given name: " + notificationDefinition.Name + ". Notification names must be unique!");
            }

            _notificationDefinitions[notificationDefinition.Name] = notificationDefinition;
        }

        public NotificationDefinition Get(string name)
        {
            var definition = GetOrNull(name);
            if (definition == null)
            {
                throw new AbpException("There is no notification definition with given name: " + name);
            }

            return definition;
        }

        public NotificationDefinition GetOrNull(string name)
        {
            return _notificationDefinitions.GetOrDefault(name);
        }

        public IReadOnlyList<NotificationDefinition> GetAll()
        {
            return _notificationDefinitions.Values.ToImmutableList();
        }

        public async Task<IReadOnlyList<NotificationDefinition>> GetAllAvailableAsync(int? tenantId, long userId)
        {
            var availableDefinitions = new List<NotificationDefinition>();

            foreach (var notificationDefinition in GetAll())
            {
                if (notificationDefinition.PermissionDependency != null &&
                    !await notificationDefinition.PermissionDependency.IsSatisfiedAsync(_permissionDependencyContext))
                {
                    continue;
                }

                if (notificationDefinition.FeatureDependency != null &&
                    !await notificationDefinition.FeatureDependency.IsSatisfiedAsync(_featureDependencyContext))
                {
                    continue;
                }

                availableDefinitions.Add(notificationDefinition);
            }

            return availableDefinitions.ToImmutableList();
        }
    }
}