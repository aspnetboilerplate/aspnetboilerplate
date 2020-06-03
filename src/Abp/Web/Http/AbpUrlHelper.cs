using System;
using Abp.Dependency;
using JetBrains.Annotations;

namespace Abp.Web.Http
{
    public class AbpUrlHelper : IUrlHelper, ISingletonDependency
    {
        /// <summary>
        /// Extract local path and query from <paramref name="url"/>.
        /// If <paramref name="url"/> is an absolute url, 
        /// <paramref name="localHostName"/> and <paramref name="localPort"/> will be used to check against the host and port in <paramref name="url"/> if any.
        /// </summary>
        /// <param name="url">absolute or relative url</param>
        /// <param name="localHostName"></param>
        /// <param name="localPort"></param>
        /// <returns></returns>
        public virtual string LocalPathAndQuery([NotNull] string url, [CanBeNull] string localHostName = null, [CanBeNull] int? localPort = null)
        {
            Check.NotNull(url, nameof(url));

            var uri = ParseWithUriBuilder(url) ?? ParseWithUri(url);
            if (uri != null && uri.IsWellFormedOriginalString())
            {
                if (uri.IsAbsoluteUri)
                {
                    var isValidScheme = uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
                    var isSameHost = string.Equals(localHostName, uri.Host, StringComparison.OrdinalIgnoreCase);
                    var isSamePort = localPort == uri.Port || (localPort == null && uri.IsDefaultPort);
                    if (isValidScheme && isSameHost && isSamePort)
                    {
                        return uri.PathAndQuery;
                    }
                }
                else if (!uri.IsAbsoluteUri)
                {
                    return uri.OriginalString;
                }
            }
            return null;
        }

        protected virtual Uri ParseWithUriBuilder(string url)
        {
            try
            {
                return new UriBuilder(url).Uri;
            }
            catch (UriFormatException)
            {
                return null;
            }
        }

        protected virtual Uri ParseWithUri(string url)
        {
            if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out Uri returnUri))
            {
                return returnUri;
            }
            return null;
        }
    }
}