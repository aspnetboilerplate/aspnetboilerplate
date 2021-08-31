using System;
using System.Collections.ObjectModel;

namespace Abp.Web.Results.Filters
{
    public class WrapResultFilterCollection: Collection<IWrapResultFilter>
    {
        /// <summary>
        /// Returns whether to apply the filter. Stores filter result on <paramref name="wrapOnSuccess"/>. Checks the filters in the list sequentially.
        /// <para>for example, the first element in the list has priority over the second element.</para>
        /// </summary>
        /// <param name="url"></param>
        /// <param name="wrapOnSuccess"></param>
        /// <returns></returns>
        public bool HasFilterForWrapOnSuccess(string url, out bool wrapOnSuccess)
        {
            for (var index = 0; index < Items.Count; index++)
            {
                if (Items[index].HasFilterForWrapOnSuccess(url, out wrapOnSuccess))
                {
                    return true;
                }
            }

            wrapOnSuccess = false;
            return false;
        }
        
        /// <summary>
        /// Returns whether to apply the filter. Stores filter result on <paramref name="wrapOnError"/>. Checks the filters in the list sequentially.
        /// <para>for example, the first element in the list has priority over the second element.</para>
        /// </summary>
        /// <param name="url"></param>
        /// <param name="wrapOnError"></param>
        /// <returns></returns>
        public bool HasFilterForWrapOnError(string url, out bool wrapOnError)
        {
            for (var index = 0; index < Items.Count; index++)
            {
                if (Items[index].HasFilterForWrapOnError(url, out wrapOnError))
                {
                    return true;
                }
            }

            wrapOnError = false;
            return false;
        }
    }
}