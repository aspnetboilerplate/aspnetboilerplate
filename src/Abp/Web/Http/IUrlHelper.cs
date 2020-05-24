using JetBrains.Annotations;

namespace Abp.Web.Http
{
    public interface IUrlHelper
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
        string LocalPathAndQuery([NotNull] string url, [CanBeNull] string localHostName = null, [CanBeNull] int? localPort = null);
    }
}