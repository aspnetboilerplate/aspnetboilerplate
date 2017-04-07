using Abp.Extensions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;

namespace Abp.AspNetCore
{
    public static class AbpUrlHelper
    {
        public static bool IsLocalUrl([NotNull] HttpRequest request, [NotNull] string url)
        {
            Check.NotNull(request, nameof(request));
            Check.NotNull(url, nameof(url));

            return IsRelativeLocalUrl(url) || url.StartsWith(GetLocalUrlRoot(request));
        }

        private static string GetLocalUrlRoot(HttpRequest request)
        {
            return request.Scheme + "://" + request.Host;
        }

        private static bool IsRelativeLocalUrl(string url)
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