using System.Collections.Generic;

namespace Abp.Web.Security
{
    public class CsrfConfiguration : ICsrfConfiguration
    {
        public string TokenCookieName { get; set; }

        public string TokenHeaderName { get; set; }

        public bool IsEnabled { get; set; }

        public HashSet<HttpVerb> IgnoredHttpVerbs { get; }

        public CsrfConfiguration()
        {
            TokenCookieName = "XSRF-TOKEN";
            TokenHeaderName = "X-XSRF-TOKEN";
            IsEnabled = true;
            IgnoredHttpVerbs = new HashSet<HttpVerb> { HttpVerb.Get, HttpVerb.Head };
        }
    }
}