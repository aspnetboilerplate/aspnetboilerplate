using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.Webhooks
{
    public interface IWebhookDefinitionManager
    {
        /// <summary>
        /// Adds the specified webhook definition. Throws exception if it is already added
        /// </summary>
        void Add(WebhookDefinition webhookDefinition);

        /// <summary>
        /// Gets a webhook definition by name.
        /// Returns null if there is no webhook definition with given name.
        /// </summary>
        WebhookDefinition GetOrNull(string name);

        /// <summary>
        /// Gets a webhook definition by name.
        /// Throws exception if there is no webhook definition with given name.
        /// </summary>
        WebhookDefinition Get(string name);

        /// <summary>
        /// Gets all webhook definitions.
        /// </summary>
        IReadOnlyList<WebhookDefinition> GetAll();

        /// <summary>
        /// Remove webhook with given name
        /// </summary>
        /// <param name="name">webhook definition name</param>
        bool Remove(string name);

        /// <summary>
        /// Checks if webhook definitions contains given webhook name 
        /// </summary>
        bool Contains(string name);

        /// <summary>
        /// Checks if given webhook name is available for given tenant.
        /// </summary>
        Task<bool> IsAvailableAsync(int? tenantId, string name);

        /// <summary>
        /// Checks if given webhook name is available for given tenant.
        /// </summary>
        bool IsAvailable(int? tenantId, string name);
    }
}
