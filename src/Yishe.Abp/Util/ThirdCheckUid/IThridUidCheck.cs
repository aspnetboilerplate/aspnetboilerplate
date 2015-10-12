using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yishe.Abp.Util.ThirdCheckUid
{
    public interface IThridUidCheck : ITransientDependency
    {
        bool CheckById(int type, string uid, string token, string weixinopenid);

    }
}
