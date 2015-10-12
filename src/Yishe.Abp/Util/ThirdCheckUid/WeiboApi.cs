using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Yishe.Abp.Util.ThirdCheckUid
{
    public class WeiboApi
    {

        /// <summary>
		/// OAuth授权之后，获取授权用户的UID 
		/// </summary>
		/// <returns>JSON</returns>
		public string GetUID(string tk)
        {
            weiboClient client = new weiboClient();
            return (client.GetCommand("account/get_uid", new WeiboParameter("access_token", tk)));
        }





    }
    internal enum RequestMethod
    {
        Get,
        Post
    }

    /// <summary>
    /// 微博操作类
    /// </summary>
    public class weiboClient
    {
        private const string BASE_URL = "https://api.weibo.com/2/";

        /// <summary>
        /// 微博工具类
        /// </summary>
        public static class Utility
        {
            /// <summary>
            /// 将微博时间转换为DateTime
            /// </summary>
            /// <param name="dateString">微博时间字符串</param>
            /// <returns>DateTime</returns>
            public static DateTime ParseUTCDate(string dateString)
            {
                CultureInfo provider = CultureInfo.InvariantCulture;

                DateTime dt = DateTime.ParseExact(dateString, "ddd MMM dd HH:mm:ss zzz yyyy", provider);

                return dt;
            }




            internal static string BuildQueryString(Dictionary<string, string> parameters)
            {
                var pairs = new List<string>();
                foreach (var item in parameters)
                {
                    if (string.IsNullOrEmpty(item.Value))
                    {
                        continue;
                    }

                    pairs.Add(string.Format("{0}={1}", Uri.EscapeDataString(item.Key), Uri.EscapeDataString(item.Value)));
                }

                return string.Join("&", pairs.ToArray());
            }

            internal static string BuildQueryString(params WeiboParameter[] parameters)
            {
                var pairs = new List<string>();
                foreach (WeiboParameter item in parameters)
                {
                    if (item.IsBinaryData)
                    {
                        continue;
                    }

                    string value = string.Format("{0}", item.Value);
                    if (!string.IsNullOrEmpty(value))
                    {
                        pairs.Add(string.Format("{0}={1}", Uri.EscapeDataString(item.Name), Uri.EscapeDataString(value)));
                    }
                }

                return string.Join("&", pairs.ToArray());
            }

            internal static string GetBoundary()
            {
                string pattern = "abcdefghijklmnopqrstuvwxyz0123456789";
                var boundaryBuilder = new StringBuilder();
                var rnd = new Random();
                for (int i = 0; i < 10; i++)
                {
                    int index = rnd.Next(pattern.Length);
                    boundaryBuilder.Append(pattern[index]);
                }
                return boundaryBuilder.ToString();
            }


        }



        /// <summary>
        /// 用GET方式发送微博指令
        /// </summary>
        /// <param name="command">微博命令。命令例如：statuses/public_timeline。详见官方API文档。</param>
        /// <param name="parameters">参数表</param>
        /// <returns></returns>
        public string GetCommand(string command, params WeiboParameter[] parameters)
        {
            return this.Http(command, RequestMethod.Get, parameters);
        }

        /// <summary>
        /// 用GET方式发送微博指令
        /// </summary>
        /// <param name="command">微博命令。命令例如：statuses/public_timeline。详见官方API文档。</param>
        /// <param name="parameters">参数表</param>
        /// <returns></returns>
        public string GetCommand(string command, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            var list = new List<WeiboParameter>();
            foreach (var item in parameters)
            {
                list.Add(new WeiboParameter(item.Key, item.Value));
            }
            return this.Http(command, RequestMethod.Get, list.ToArray());
        }

        private string Http(string command, RequestMethod method, params WeiboParameter[] parameters)
        {
            string url = string.Empty;

            if (command.StartsWith("http://") || command.StartsWith("https://"))
            {
                url = command;
            }
            else
            {
                url = string.Format("{0}{1}.json", BASE_URL, command);
            }
            return Request(url, method, parameters);
        }
        internal string Request(string url, RequestMethod method = RequestMethod.Get, params WeiboParameter[] parameters)
        {
            string rawUrl = string.Empty;
            var uri = new UriBuilder(url);
            string result = string.Empty;

            bool multi = false;

            multi = parameters.Count(p => p.IsBinaryData) > 0;

            switch (method)
            {
                case RequestMethod.Get:
                    {
                        uri.Query = Utility.BuildQueryString(parameters);
                    }
                    break;
                case RequestMethod.Post:
                    {
                        if (!multi)
                        {
                            uri.Query = Utility.BuildQueryString(parameters);
                        }
                    }
                    break;
            }



            var http = WebRequest.Create(uri.Uri) as HttpWebRequest;
            http.ServicePoint.Expect100Continue = false;
            http.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0)";
            http.Method = "GET";


            try
            {
                using (WebResponse response = http.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        try
                        {
                            result = reader.ReadToEnd();
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        finally
                        {
                            reader.Close();
                        }
                    }

                    response.Close();
                }
            }
            catch (Exception ex)
            {

                return "";
            }




            return result;
        }

    }

    /// <summary>
	/// 微博API参数
	/// </summary>
	public class WeiboParameter
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WeiboParameter()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">key</param>
        /// <param name="value">value</param>
        public WeiboParameter(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">key</param>
        /// <param name="value">value</param>
        public WeiboParameter(string name, bool value)
        {
            this.Name = name;
            this.Value = value ? "1" : "0";
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">key</param>
        /// <param name="value">value</param>
        public WeiboParameter(string name, int value)
        {
            this.Name = name;
            this.Value = string.Format("{0}", value);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">key</param>
        /// <param name="value">value</param>
        public WeiboParameter(string name, long value)
        {
            this.Name = name;
            this.Value = string.Format("{0}", value);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">key</param>
        /// <param name="value">value</param>
        public WeiboParameter(string name, byte[] value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">key</param>
        /// <param name="value">value</param>
        public WeiboParameter(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 参数值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 是否为二进制参数（如图片、文件等）
        /// </summary>
        public bool IsBinaryData
        {
            get
            {
                if (this.Value != null && this.Value.GetType() == typeof(byte[]))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
