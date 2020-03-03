using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Dependency;
using Abp.Extensions;
using Abp.UI.Inputs;

namespace Abp.DynamicEntityParameters
{
    public class DynamicEntityParameterDefinitionManager : IDynamicEntityParameterDefinitionManager, ISingletonDependency
    {
        private readonly IDynamicEntityParameterConfiguration _dynamicEntityParametersConfiguration;
        private readonly IocManager _iocManager;

        private readonly Dictionary<string, Type> _allowedInputTypes;

        public DynamicEntityParameterDefinitionManager(
            IDynamicEntityParameterConfiguration dynamicEntityParametersConfiguration,
            IocManager iocManager
            )
        {
            _dynamicEntityParametersConfiguration = dynamicEntityParametersConfiguration;
            _iocManager = iocManager;

            _allowedInputTypes = new Dictionary<string, Type>();
        }

        public void Initialize()
        {
            var context = _iocManager.Resolve<IDynamicEntityParameterDefinitionContext>();
            context.Manager = this;

            foreach (var providerType in _dynamicEntityParametersConfiguration.Providers)
            {
                using (var provider = _iocManager.ResolveAsDisposable<DynamicEntityParameterDefinitionProvider>(providerType))
                {
                    provider.Object.SetWebhooks(context);
                }
            }
        }

        public void AddAllowedInputType<TInputType>() where TInputType : IInputType
        {
            IInputType inputType = (IInputType)Activator.CreateInstance(typeof(TInputType));

            if (inputType.Name.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(typeof(TInputType).FullName + "/" + nameof(inputType.Name));
            }

            if (_allowedInputTypes.ContainsKey(inputType.Name))
            {
                throw new Exception($"Input types must be unique.There is already an input type named \"{inputType.Name}\"");
            }

            _allowedInputTypes.Add(inputType.Name, typeof(TInputType));
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
    }
}
