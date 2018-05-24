using System.Threading.Tasks;

namespace Abp.Web.Authorization
{
    /// <summary>
    /// This class is used to build authorization script.
    /// </summary>
    public interface IAuthorizationScriptManager
    {
        /// <summary>
        /// Gets JavaScript that contains all authorization information.
        /// </summary>
        Task<string> GetScriptAsync();
    }
}
