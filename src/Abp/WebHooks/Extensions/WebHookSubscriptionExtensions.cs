using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Json;

namespace Abp.WebHooks
{
    public static class WebHookSubscriptionExtensions
    {
        /// <summary>
        /// Return List of subscribed webhooks definitions <see cref="WebHookSubscriptionInfo.WebHookDefinitions"/>
        /// </summary>
        /// <returns></returns>
        public static List<string> GetWebHookDefinitions(this WebHookSubscriptionInfo webHookSubscription)
        {
            if (string.IsNullOrWhiteSpace(webHookSubscription.WebHookDefinitions))
            {
                return new List<string>();
            }

            return webHookSubscription.WebHookDefinitions.FromJsonString<List<string>>();
        }

        /// <summary>
        /// Adds webhook subscription to <see cref="WebHookSubscriptionInfo.WebHookDefinitions"/> if not exists
        /// </summary>
        /// <param name="webHookSubscription"></param>
        /// <param name="name">webhook unique name</param>
        public static void AddWebHookDefinition(this WebHookSubscriptionInfo webHookSubscription, string name)
        {
            name = name.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name), $"{nameof(name)} can not be null, empty or whitespace!");
            }

            var webHookDefinitions = webHookSubscription.GetWebHookDefinitions();
            if (webHookDefinitions.Contains(name))
            {
                return;
            }

            webHookDefinitions.Add(name);
            webHookSubscription.WebHookDefinitions = webHookDefinitions.ToJsonString();
        }

        /// <summary>
        ///  Removes webhook subscription from <see cref="WebHookSubscriptionInfo.WebHookDefinitions"/> if exists
        /// </summary>
        /// <param name="webHookSubscription"></param>
        /// <param name="name">webhook unique name</param>
        public static void RemoveWebHookDefinition(this WebHookSubscriptionInfo webHookSubscription, string name)
        {
            name = name.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name), $"{nameof(name)} can not be null, empty or whitespace!");
            }

            var webHookDefinitions = webHookSubscription.GetWebHookDefinitions();
            if (!webHookDefinitions.Contains(name))
            {
                return;
            }

            webHookDefinitions.Remove(name);
            webHookSubscription.WebHookDefinitions = webHookDefinitions.ToJsonString();
        }

        /// <summary>
        /// Clears all <see cref="WebHookSubscriptionInfo.WebHookDefinitions"/> 
        /// </summary>
        /// <param name="webHookSubscription"></param> 
        public static void ClearAllSubscriptions(this WebHookSubscriptionInfo webHookSubscription)
        {
            webHookSubscription.WebHookDefinitions = null;
        }

        public static WebHookSubscription ToWebHookSubscription(this WebHookSubscriptionInfo webHookSubscriptionInfo)
        {
            return new WebHookSubscription()
            {
                Id = webHookSubscriptionInfo.Id,
                TenantId = webHookSubscriptionInfo.TenantId,
                IsActive = webHookSubscriptionInfo.IsActive,
                Secret = webHookSubscriptionInfo.Secret,
                WebHookUri = webHookSubscriptionInfo.WebHookUri,
                WebHookDefinitions = webHookSubscriptionInfo.GetWebHookDefinitions().ToList(),
                Headers = webHookSubscriptionInfo.GetWebHookHeaders()
            };
        }

        public static WebHookSubscriptionInfo ToWebHookSubscriptionInfo(this WebHookSubscription webHookSubscription)
        {
            return new WebHookSubscriptionInfo()
            {
                Id = webHookSubscription.Id,
                TenantId = webHookSubscription.TenantId,
                IsActive = webHookSubscription.IsActive,
                Secret = webHookSubscription.Secret,
                WebHookUri = webHookSubscription.WebHookUri,
                WebHookDefinitions = webHookSubscription.WebHookDefinitions.ToJsonString(),
                Headers = webHookSubscription.Headers.ToJsonString()
            };
        }

        /// <summary>
        /// if subscribed to given webhook
        /// </summary>
        /// <returns></returns>
        public static bool IsSubscribed(this WebHookSubscriptionInfo webHookSubscription, string webHookName)
        {
            if (string.IsNullOrWhiteSpace(webHookSubscription.WebHookDefinitions))
            {
                return false;
            }

            return webHookSubscription.GetWebHookDefinitions().Contains(webHookName);
        }

        /// <summary>
        /// if subscribed to given webhook
        /// </summary>
        /// <returns></returns>
        public static bool IsSubscribed(this WebHookSubscription webHookSubscription, string webHookName)
        {
            if (webHookSubscription.WebHookDefinitions == null || webHookSubscription.WebHookDefinitions.Count == 0)
            {
                return false;
            }

            return webHookSubscription.WebHookDefinitions.Contains(webHookName);
        }

        /// <summary>
        /// Returns additional webhook headers <see cref="WebHookSubscriptionInfo.Headers"/>
        /// </summary>
        /// <returns></returns>
        public static IDictionary<string, string> GetWebHookHeaders(this WebHookSubscriptionInfo webHookSubscription)
        {
            if (string.IsNullOrWhiteSpace(webHookSubscription.Headers))
            {
                return new Dictionary<string, string>();
            }

            return webHookSubscription.Headers.FromJsonString<Dictionary<string, string>>();
        }

        /// <summary>
        /// Adds webhook subscription to <see cref="WebHookSubscriptionInfo.WebHookDefinitions"/> if not exists
        /// </summary>
        /// <param name="webHookSubscription"></param>
        /// <param name="header">header to add</param>
        public static void AddWebHookHeader(this WebHookSubscriptionInfo webHookSubscription, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key) )
            {
                throw new ArgumentNullException(nameof(key), $"{nameof(key)} can not be null, empty or whitespace!");
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value), $"{nameof(value)} can not be null, empty or whitespace!");
            }

            var headers = webHookSubscription.GetWebHookHeaders();
            headers[key] = value;

            webHookSubscription.Headers = headers.ToJsonString();
        }

        /// <summary>
        /// Adds webhook subscription to <see cref="WebHookSubscriptionInfo.WebHookDefinitions"/> if not exists
        /// </summary>
        /// <param name="webHookSubscription"></param>
        /// <param name="header">Key of header</param>
        public static void RemoveWebHookHeader(this WebHookSubscriptionInfo webHookSubscription, string header)
        {
            if (string.IsNullOrWhiteSpace(header))
            {
                throw new ArgumentNullException(nameof(header), $"{nameof(header)} can not be null, empty or whitespace!");
            }

            var headers = webHookSubscription.GetWebHookHeaders();

            if (!headers.ContainsKey(header))
            {
                return;
            }

            headers.Remove(header);

            webHookSubscription.Headers = headers.ToJsonString();
        }

    }
}
