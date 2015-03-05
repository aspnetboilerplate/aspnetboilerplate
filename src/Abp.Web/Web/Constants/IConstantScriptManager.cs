using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Web.Constants
{
    /// <summary>
    /// Define interface to get constants and enumerations injected into a script
    /// </summary>
    public interface IConstantScriptManager
    {
        /// <summary>
        /// Gets Javascript that contains setting values.
        /// </summary>
        string GetScript();
    }
}
