using Abp.Web;

namespace Abp.WebApi.Controllers.Dynamic.Builders
{
    /// <summary>
    /// NOTE: This is not used (as all members are private)
    /// </summary>
    internal static class DynamicApiVerbHelper
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