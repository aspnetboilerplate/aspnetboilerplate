using System.Collections.Generic;
using Abp.UI.Inputs;

namespace Abp.DynamicEntityParameters
{
    public interface IDynamicEntityParameterDefinitionManager
    {
        /// <summary>
        /// Adds the specified inputType to allowed list. Throws exception if it is already added
        /// </summary>
        void AddAllowedInputType<TInputType>() where TInputType : IInputType;

        /// <summary>
        /// Gets a Input Type by name.
        /// Returns null if there is no webhook definition with given name.
        /// </summary>
        IInputType GetOrNullAllowedInputType(string name);

        /// <summary>
        /// Gets all input type names
        /// </summary>
        List<string> GetAllAllowedInputTypeNames();

        /// <summary>
        /// Gets all input types
        /// </summary>
        List<IInputType> GetAllAllowedInputTypes();

        /// <summary>
        /// Returns if allowed input types contains the given name
        /// </summary>
        bool ContainsInputType(string name);
    }
}
