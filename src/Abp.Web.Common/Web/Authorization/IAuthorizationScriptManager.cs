using System.Threading.Tasks;

namespace Abp.Web.Authorization
{
    /// <summary>
    /// This class is used to build authorization script.
    /// </summary>
    public interface IAuthorizationScriptManager
    {
        /// <summary>
        /// Gets Javascript that contains all authorization information.
        /// </summary>
        Task<string> GetScriptAsync();
    }
}
