using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;

namespace Abp.WebHooks
{
    public static class WebHookSubscriptionExtensions
    {
        /// <summary>
        /// Return is of subscribed webhooks definitions <see cref="WebHookSubscriptionInfo.WebHookDefinitions"/>
        /// </summary>
        /// <returns></returns>
        public static ImmutableList<string> GetWebHookDefinitions(this WebHookSubscriptionInfo webHookSubscription)
        {
            if (string.IsNullOrWhiteSpace(webHookSubscription.WebHookDefinitions))
            {
                return new Collection<string>().ToImmutableList();
            }

            return webHookSubscription.WebHookDefinitions.Split(';').Where(s => !string.IsNullOrWhiteSpace(s)).ToImmutableList();
        }

        /// <summary>
        /// Adds webhook subscription to <see cref="WebHookSubscriptionInfo.WebHookDefinitions"/> if not exists
        /// </summary>
        /// <param name="webHookSubscription"></param>
        /// <param name="name">webhook unique name</param>
        public static void AddWebHookDefinition(this WebHookSubscriptionInfo webHookSubscription, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name), $"{nameof(name)} can not be null, empty or whitespace!");
            }

            if (name.Contains(";"))
            {
                throw new ArgumentException($"{nameof(name)} can not contain ';'", nameof(name));
            }

            if (webHookSubscription.GetWebHookDefinitions().Contains(name))
            {
                return;
            }

            webHookSubscription.WebHookDefinitions += ";" + name.Trim();
        }

        /// <summary>
        ///  Removes webhook subscription from <see cref="WebHookSubscriptionInfo.WebHookDefinitions"/> if exists
        /// </summary>
        /// <param name="webHookSubscription"></param>
        /// <param name="name">webhook unique name</param>
        public static void RemoveWebHookDefinition(this WebHookSubscriptionInfo webHookSubscription, string name)
        {
            if (string.IsNullOrWhiteSpace(name) || !webHookSubscription.GetWebHookDefinitions().Contains(name))
            {
                return;
            }

            webHookSubscription.WebHookDefinitions = webHookSubscription.WebHookDefinitions.Replace(";" + name, "");
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
                UserId = webHookSubscriptionInfo.UserId,
                IsActive = webHookSubscriptionInfo.IsActive,
                Secret = webHookSubscriptionInfo.Secret,
                WebHookUri = webHookSubscriptionInfo.WebHookUri,
                WebHookDefinitions = webHookSubscriptionInfo.GetWebHookDefinitions().ToList()
            };
        }

        public static WebHookSubscriptionInfo ToWebHookSubscriptionInfo(this WebHookSubscription webHookSubscriptionInfo)
        {
            return new WebHookSubscriptionInfo()
            {
                Id = webHookSubscriptionInfo.Id,
                TenantId = webHookSubscriptionInfo.TenantId,
                UserId = webHookSubscriptionInfo.UserId,
                IsActive = webHookSubscriptionInfo.IsActive,
                Secret = webHookSubscriptionInfo.Secret,
                WebHookUri = webHookSubscriptionInfo.WebHookUri,
                WebHookDefinitions = ";" + string.Join(";", webHookSubscriptionInfo.WebHookDefinitions)
            };
        }
    }
}
