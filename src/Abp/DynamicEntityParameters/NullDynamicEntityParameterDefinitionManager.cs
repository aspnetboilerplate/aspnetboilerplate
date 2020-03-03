using System.Collections.Generic;
using Abp.UI.Inputs;

namespace Abp.DynamicEntityParameters
{
    public class NullDynamicEntityParameterDefinitionManager : IDynamicEntityParameterDefinitionManager
    {
        public static NullDynamicEntityParameterDefinitionManager Instance = new NullDynamicEntityParameterDefinitionManager();

        public void AddAllowedInputType<TInputType>() where TInputType : IInputType
        {
        }

        public IInputType GetOrNullAllowedInputType(string name)
        {
            return null;
        }

        public List<string> GetAllAllowedInputTypeNames()
        {
            return new List<string>();
        }

        public List<IInputType> GetAllAllowedInputTypes()
        {
            return new List<IInputType>();
        }

        public bool ContainsInputType(string name)
        {
            return false;
        }
    }
}
