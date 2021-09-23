namespace Abp.Web.Results.Filters
{
    public interface IWrapResultFilter
    {
        /// <summary>
        /// Returns whether to apply the filter. Stores filter result on <paramref name="wrapOnSuccess"/>.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="wrapOnSuccess"></param>
        /// <returns></returns>
        bool HasFilterForWrapOnSuccess(string url, out bool wrapOnSuccess);
        
        /// <summary>
        /// Returns whether to apply the filter. Stores filter result on <paramref name="wrapOnError"/>.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="wrapOnError"></param>
        /// <returns></returns>
        bool HasFilterForWrapOnError(string url, out bool wrapOnError);
    }
}