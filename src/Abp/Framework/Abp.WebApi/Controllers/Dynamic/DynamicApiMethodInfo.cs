using System.Reflection;

namespace Abp.WebApi.Controllers.Dynamic
{
    /// <summary>
    /// Used to store an action information.
    /// </summary>
    internal class DynamicApiActionInfo
    {
        /// <summary>
        /// Name of the action in the controller.
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// The method which will be invoked when this action is called.
        /// </summary>
        public MethodInfo Method { get; set; }

        /// <summary>
        /// The HTTP verb that is used to call this action.
        /// </summary>
        public HttpVerb Verb { get; set; }

        /// <summary>
        /// Createa a new <see cref="DynamicApiActionInfo"/> object.
        /// </summary>
        /// <param name="actionName">Name of the action in the controller</param>
        /// <param name="method">The method which will be invoked when this action is called</param>
        public DynamicApiActionInfo(string actionName, MethodInfo method)
        {
            ActionName = actionName;
            Method = method;
            Verb = HttpVerb.Post; //TODO: Get default from somewhere else!
        }
    }
}