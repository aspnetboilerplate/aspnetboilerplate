using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.WebHooks
{
    public interface IWebHookDefinitionManager
    {
        /// <summary>
        /// Adds the specified webhook definition. Throws exception if it is already added
        /// </summary>
        void Add(WebHookDefinition webHookDefinition);

        /// <summary>
        /// Adds the specified webhook definition.If there is already a definition with given name, replaces it with current one
        /// </summary>
        void AddOrReplace(WebHookDefinition webHookDefinition);

        /// <summary>
        /// Gets a webhook definition by name.
        /// Returns null if there is no webhook definition with given name.
        /// </summary>
        WebHookDefinition GetOrNull(string name);

        /// <summary>
        /// Gets a webhook definition by name.
        /// Throws exception if there is no webhook definition with given name.
        /// </summary>
        WebHookDefinition Get(string name);

        /// <summary>
        /// Gets all webhook definitions.
        /// </summary>
        IReadOnlyList<WebHookDefinition> GetAll();

        /// <summary>
        /// Remove webhook with given name
        /// </summary>
        /// <param name="name">webhook definition name</param>
        /// <param name="webHookDefinition"></param>
        bool TryRemove(string name, out WebHookDefinition webHookDefinition);

        /// <summary>
        /// Checks if webhook definitions contains given webhook name 
        /// </summary>
        bool Contains(string name);

        /// <summary>
        /// Checks if given webhook name is available for given user.
        /// </summary>
        Task<bool> IsAvailableAsync(UserIdentifier user, string name);

        /// <summary>
        /// Checks if given webhook name is available for given user.
        /// </summary>
        bool IsAvailable(UserIdentifier user, string name);
    }
}
