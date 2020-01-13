using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Json;

namespace Abp.Webhooks
{
    public static class WebhookSubscriptionExtensions
    {
        /// <summary>
        /// Return List of subscribed webhooks definitions <see cref="WebhookSubscriptionInfo.WebhookDefinitions"/>
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSubscribedWebhooks(this WebhookSubscriptionInfo webhookSubscription)
        {
            if (string.IsNullOrWhiteSpace(webhookSubscription.WebhookDefinitions))
            {
                return new List<string>();
            }

            return webhookSubscription.WebhookDefinitions.FromJsonString<List<string>>();
        }

        /// <summary>
        /// Adds webhook subscription to <see cref="WebhookSubscriptionInfo.WebhookDefinitions"/> if not exists
        /// </summary>
        /// <param name="webhookSubscription"></param>
        /// <param name="name">webhook unique name</param>
        public static void SubscribeWebhook(this WebhookSubscriptionInfo webhookSubscription, string name)
        {
            name = name.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name), $"{nameof(name)} can not be null, empty or whitespace!");
            }

            var webhookDefinitions = webhookSubscription.GetSubscribedWebhooks();
            if (webhookDefinitions.Contains(name))
            {
                return;
            }

            webhookDefinitions.Add(name);
            webhookSubscription.WebhookDefinitions = webhookDefinitions.ToJsonString();
        }

        /// <summary>
        ///  Removes webhook subscription from <see cref="WebhookSubscriptionInfo.WebhookDefinitions"/> if exists
        /// </summary>
        /// <param name="webhookSubscription"></param>
        /// <param name="name">webhook unique name</param>
        public static void UnsubscribeWebhook(this WebhookSubscriptionInfo webhookSubscription, string name)
        {
            name = name.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name), $"{nameof(name)} can not be null, empty or whitespace!");
            }

            var webhookDefinitions = webhookSubscription.GetSubscribedWebhooks();
            if (!webhookDefinitions.Contains(name))
            {
                return;
            }

            webhookDefinitions.Remove(name);
            webhookSubscription.WebhookDefinitions = webhookDefinitions.ToJsonString();
        }

        /// <summary>
        /// Clears all <see cref="WebhookSubscriptionInfo.WebhookDefinitions"/> 
        /// </summary>
        /// <param name="webhookSubscription"></param> 
        public static void RemoveAllSubscribedWebhooks(this WebhookSubscriptionInfo webhookSubscription)
        {
            webhookSubscription.WebhookDefinitions = null;
        }

        public static WebhookSubscription ToWebhookSubscription(this WebhookSubscriptionInfo webhookSubscriptionInfo)
        {
            return new WebhookSubscription()
            {
                Id = webhookSubscriptionInfo.Id,
                TenantId = webhookSubscriptionInfo.TenantId,
                IsActive = webhookSubscriptionInfo.IsActive,
                Secret = webhookSubscriptionInfo.Secret,
                WebhookUri = webhookSubscriptionInfo.WebhookUri,
                WebhookDefinitions = webhookSubscriptionInfo.GetSubscribedWebhooks().ToList(),
                Headers = webhookSubscriptionInfo.GetWebhookHeaders()
            };
        }

        public static WebhookSubscriptionInfo ToWebhookSubscriptionInfo(this WebhookSubscription webhookSubscription)
        {
            return new WebhookSubscriptionInfo()
            {
                Id = webhookSubscription.Id,
                TenantId = webhookSubscription.TenantId,
                IsActive = webhookSubscription.IsActive,
                Secret = webhookSubscription.Secret,
                WebhookUri = webhookSubscription.WebhookUri,
                WebhookDefinitions = webhookSubscription.WebhookDefinitions.ToJsonString(),
                Headers = webhookSubscription.Headers.ToJsonString()
            };
        }

        /// <summary>
        /// if subscribed to given webhook
        /// </summary>
        /// <returns></returns>
        public static bool IsSubscribed(this WebhookSubscriptionInfo webhookSubscription, string webhookName)
        {
            if (string.IsNullOrWhiteSpace(webhookSubscription.WebhookDefinitions))
            {
                return false;
            }

            return webhookSubscription.GetSubscribedWebhooks().Contains(webhookName);
        }

        /// <summary>
        /// if subscribed to given webhook
        /// </summary>
        /// <returns></returns>
        public static bool IsSubscribed(this WebhookSubscription webhookSubscription, string webhookName)
        {
            if (webhookSubscription.WebhookDefinitions == null || webhookSubscription.WebhookDefinitions.Count == 0)
            {
                return false;
            }

            return webhookSubscription.WebhookDefinitions.Contains(webhookName);
        }

        /// <summary>
        /// Returns additional webhook headers <see cref="WebhookSubscriptionInfo.Headers"/>
        /// </summary>
        /// <returns></returns>
        public static IDictionary<string, string> GetWebhookHeaders(this WebhookSubscriptionInfo webhookSubscription)
        {
            if (string.IsNullOrWhiteSpace(webhookSubscription.Headers))
            {
                return new Dictionary<string, string>();
            }

            return webhookSubscription.Headers.FromJsonString<Dictionary<string, string>>();
        }

        /// <summary>
        /// Adds webhook subscription to <see cref="WebhookSubscriptionInfo.WebhookDefinitions"/> if not exists
        /// </summary>
        public static void AddWebhookHeader(this WebhookSubscriptionInfo webhookSubscription, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key) )
            {
                throw new ArgumentNullException(nameof(key), $"{nameof(key)} can not be null, empty or whitespace!");
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value), $"{nameof(value)} can not be null, empty or whitespace!");
            }

            var headers = webhookSubscription.GetWebhookHeaders();
            headers[key] = value;

            webhookSubscription.Headers = headers.ToJsonString();
        }

        /// <summary>
        /// Adds webhook subscription to <see cref="WebhookSubscriptionInfo.WebhookDefinitions"/> if not exists
        /// </summary>
        /// <param name="webhookSubscription"></param>
        /// <param name="header">Key of header</param>
        public static void RemoveWebhookHeader(this WebhookSubscriptionInfo webhookSubscription, string header)
        {
            if (string.IsNullOrWhiteSpace(header))
            {
                throw new ArgumentNullException(nameof(header), $"{nameof(header)} can not be null, empty or whitespace!");
            }

            var headers = webhookSubscription.GetWebhookHeaders();

            if (!headers.ContainsKey(header))
            {
                return;
            }

            headers.Remove(header);

            webhookSubscription.Headers = headers.ToJsonString();
        }

    }
}
