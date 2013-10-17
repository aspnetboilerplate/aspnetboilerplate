namespace Abp.WebApi.Controllers.Dynamic.Builders
{
    /// <summary>
    /// A helper class for dynamic api controllers.
    /// </summary>
    internal static class DynamicApiHelper
    {
        public static HttpVerb GetConventionalVerbForMethodName(string methodName)
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

        public static HttpVerb GetDefaultHttpVerb()
        {
            return HttpVerb.Get;
        }
    }
}
