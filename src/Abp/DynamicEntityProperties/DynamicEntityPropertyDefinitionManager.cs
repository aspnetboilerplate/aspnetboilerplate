using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Extensions;
using Abp.UI.Inputs;

namespace Abp.DynamicEntityProperties
{
    public class DynamicEntityPropertyDefinitionManager : IDynamicEntityPropertyDefinitionManager, ISingletonDependency
    {
        private readonly IDynamicEntityPropertyConfiguration _dynamicEntityPropertiesConfiguration;
        private readonly IocManager _iocManager;

        private readonly Dictionary<string, Type> _allowedInputTypes;

        private readonly HashSet<string> _entities;

        public DynamicEntityPropertyDefinitionManager(
            IDynamicEntityPropertyConfiguration dynamicEntityPropertiesConfiguration,
            IocManager iocManager
            )
        {
            _dynamicEntityPropertiesConfiguration = dynamicEntityPropertiesConfiguration;
            _iocManager = iocManager;

            _allowedInputTypes = new Dictionary<string, Type>();
            _entities = new HashSet<string>();
        }

        public void Initialize()
        {
            var context = _iocManager.Resolve<IDynamicEntityPropertyDefinitionContext>();
            context.Manager = this;

            foreach (var providerType in _dynamicEntityPropertiesConfiguration.Providers)
            {
                using (var provider = _iocManager.ResolveAsDisposable<DynamicEntityPropertyDefinitionProvider>(providerType))
                {
                    provider.Object.SetDynamicEntityProperties(context);
                }
            }
        }

        public void AddAllowedInputType<TInputType>() where TInputType : IInputType
        {
            var inputTypeName = InputTypeBase.GetName<TInputType>();

            if (inputTypeName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(typeof(TInputType).FullName + "/" + nameof(inputTypeName));
            }

            if (_allowedInputTypes.ContainsKey(inputTypeName))
            {
                throw new Exception($"Input types must be unique.There is already an input type named \"{inputTypeName}\"");
            }

            _allowedInputTypes.Add(inputTypeName, typeof(TInputType));
        }

        public IInputType GetOrNullAllowedInputType(string name)
        {
            return _allowedInputTypes.ContainsKey(name)
                ? (IInputType)Activator.CreateInstance(_allowedInputTypes[name])
                : null;
        }

        public List<string> GetAllAllowedInputTypeNames()
        {
            return _allowedInputTypes.Keys.ToList();
        }

        public List<IInputType> GetAllAllowedInputTypes()
        {
            return _allowedInputTypes.Select(inputType => (IInputType)Activator.CreateInstance(inputType.Value)).ToList();
        }

        public bool ContainsInputType(string name)
        {
            return _allowedInputTypes.ContainsKey(name);
        }

        public void AddEntity<TEntity>() where TEntity : IEntity<int>
        {
            AddEntity<TEntity, int>();
        }

        public void AddEntity<TEntity, TPrimaryKey>() where TEntity : IEntity<TPrimaryKey>
        {
            string entityName = typeof(TEntity).FullName;
            if (_entities.Contains(entityName))
            {
                throw new ApplicationException($"Entity already registered {entityName}");
            }

            _entities.Add(entityName);
        }

        public List<string> GetAllEntities()
        {
            return _entities.ToList();
        }

        public bool ContainsEntity(string entityFullName)
        {
            return _entities.Contains(entityFullName);
        }

        public bool ContainsEntity<TEntity, TPrimaryKey>() where TEntity : IEntity<TPrimaryKey>
        {
            return ContainsEntity(typeof(TEntity).FullName);
        }

        public bool ContainsEntity<TEntity>() where TEntity : IEntity<int>
        {
            return ContainsEntity<TEntity, int>();
        }
    }
}
