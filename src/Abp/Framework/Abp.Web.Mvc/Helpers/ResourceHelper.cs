using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Abp.Web.Mvc.Helpers
{
    public static class ResourceHelper
    {
        public static IHtmlString IncludeScript(this HtmlHelper html, string url)
        {
            if (url.StartsWith("~"))
            {
                url = url.Substring(1);
            }

            return html.Raw("<script src=\"" + url + "\" type=\"text/javascript\"></script>");
        }
    }
}
