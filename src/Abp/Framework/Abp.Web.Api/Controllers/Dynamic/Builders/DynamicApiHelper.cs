using Abp.Web;

namespace Abp.WebApi.Controllers.Dynamic.Builders
{
    /// <summary>
    /// A helper class for dynamic api controllers.
    /// NOTE: This class is not used.
    /// </summary>
    internal static class DynamicApiHelper
    {
        private static HttpVerb GetConventionalVerbForMethodName(string methodName)
        {
            if (methodName.StartsWith("Get"))
            {
                return HttpVerb.Get;
            }

            if (methodName.StartsWith("Update") || methodName.StartsWith("Put"))
            {
                return HttpVerb.Put;
            }

            if (methodName.StartsWith("Delete") || methodName.StartsWith("Remove"))
            {
                return HttpVerb.Delete;
            }

            if (methodName.StartsWith("Create") || methodName.StartsWith("Post"))
            {
                return HttpVerb.Post;
            }

            return GetDefaultHttpVerb();
        }

        private static HttpVerb GetDefaultHttpVerb()
        {
            return HttpVerb.Post;
        }
    }
}
