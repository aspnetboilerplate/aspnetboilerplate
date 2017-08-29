using System;
using Abp.Extensions;
using JetBrains.Annotations;

namespace Abp.Web
{
    public static class AbpUrlHelper
    {
        public static bool IsLocalUrl([NotNull] Uri requestUri, [NotNull] string url)
        {
            Check.NotNull(requestUri, nameof(requestUri));
            Check.NotNull(url, nameof(url));

            return IsRelativeLocalUrl(url) || url.StartsWith(GetLocalUrlRoot(requestUri));
        }

        private static string GetLocalUrlRoot(Uri requestUri)
        {
            var root = requestUri.Scheme + "://" + requestUri.Host;

            if ((string.Equals(requestUri.Scheme, "http", StringComparison.OrdinalIgnoreCase) && requestUri.Port != 80) ||
                (string.Equals(requestUri.Scheme, "https", StringComparison.OrdinalIgnoreCase) && requestUri.Port != 443))
            {
                root += ":" + requestUri.Port;
            }

            return root;
        }

        public static bool IsRelativeLocalUrl(string url)
        {
            //This code is copied from System.Web.WebPages.RequestExtensions class.

            if (url.IsNullOrEmpty())  
                return false;
            if ((int)url[0] == 47 && (url.Length == 1 || (int)url[1] != 47 && (int)url[1] != 92))
                return true;
            if (url.Length > 1 && (int)url[0] == 126)
                return (int)url[1] == 47;
            return false;
        }
    }
}