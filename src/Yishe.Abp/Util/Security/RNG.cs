using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Yishe.Abp.Core.Security
{
    /// <summary>
    /// 使用 RNGCryptoServiceProvider 产生由密码编译服务供应者 (CSP) 提供的随机数生成器。
    /// </summary>
    public static class RNG
    {
        private static RNGCryptoServiceProvider rngp = new RNGCryptoServiceProvider();
        private static byte[] rb = new byte[4];

        /// <summary>
        /// 产生一个非负数的随机数
        /// </summary>
        public static int Next()
        {
            rngp.GetBytes(rb);
            int value = BitConverter.ToInt32(rb, 0);
            if (value < 0) value = -value;
            return value;
        }
        /// <summary>
        /// 产生一个非负数且最大值 max 以下的随机数
        /// </summary>
        /// <param name="max">最大值</param>
        public static int Next(int max)
        {
            rngp.GetBytes(rb);
            int value = BitConverter.ToInt32(rb, 0);
            value = value % (max + 1);
            if (value < 0) value = -value;
            return value;
        }
        /// <summary>
        /// 产生一个非负数且最小值在 min 以上最大值在 max 以下的随机数
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        public static int Next(int min, int max)
        {
            int value = Next(max - min) + min;
            return value;
        }
    }
}
