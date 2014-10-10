using System.Collections.Generic;

namespace Abp.Application.Navigation
{
    /// <summary>
    /// Declares common interface for classes those have menu items.
    /// </summary>
    public interface IHasMenuItems
    {
        /// <summary>
        /// List of menu items.
        /// </summary>
        IList<MenuItem> Items { get; }
    }
}