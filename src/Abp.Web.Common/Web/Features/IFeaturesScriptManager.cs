using System.Threading.Tasks;

namespace Abp.Web.Features
{
    /// <summary>
    /// This class is used to build feature system script.
    /// </summary>
    public interface IFeaturesScriptManager
    {
        /// <summary>
        /// Gets Javascript that contains all feature information.
        /// </summary>
        Task<string> GetScriptAsync();
    }
}