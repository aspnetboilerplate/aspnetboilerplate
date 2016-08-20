using System.Collections.Generic;

namespace Abp.Web.Security
{
    public interface ICsrfConfiguration
    {
        /// <summary>
        /// Get/sets cookie name to transfer CSRF token between server and client.
        /// Default value: "XSRF-TOKEN".
        /// </summary>
        string TokenCookieName { get; set; }

        /// <summary>
        /// Get/sets header name to transfer CSRF token from client to the server.
        /// Default value: "X-XSRF-TOKEN". 
        /// </summary>
        string TokenHeaderName { get; set; }

        /// <summary>
        /// Used to enable/disable CSRF token security.
        /// Default: true (enabled).
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// A list of ignored HTTP verbs for CSRF token validation.
        /// Default list: Get, Head, Options, Trace.
        /// </summary>
        HashSet<HttpVerb> IgnoredHttpVerbs { get; }
    }
}