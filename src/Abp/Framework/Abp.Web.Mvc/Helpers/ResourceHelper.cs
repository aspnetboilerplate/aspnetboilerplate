using System.Web;
using System.Web.Mvc;

namespace Abp.Web.Mvc.Helpers
{
    public static class ResourceHelper
    {
        //TODO: Make shortcut to AbpViewBase
        //TODO: implement caching & versioning
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
