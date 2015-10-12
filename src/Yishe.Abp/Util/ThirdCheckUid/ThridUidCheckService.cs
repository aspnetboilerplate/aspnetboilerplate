using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yishe.Abp.Util.ThirdCheckUid
{
    public class ThridUidCheckService : IThridUidCheck
    {
        public bool CheckById(int type, string uid, string token, string weixinopenid)
        {
            if (type == 0)
            {
                //微信
                try
                {
                    var userinfo = Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetUserInfo(token, weixinopenid);
                    return userinfo.unionid == uid;
                }
                catch (Exception ex)
                {

                    return false;
                }

            }
            else if (type == 1)
            {
                //微博

                var userinfo2 = new WeiboApi().GetUID(token);
                return userinfo2.Contains(uid);

            }
            else
            {
                //QQ

                return true;
            }

            return false;
        }
    }
}
