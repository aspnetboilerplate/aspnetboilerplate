using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Web.Sessions
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISessionScriptManager
    {
        string GetScript();
    }
}
