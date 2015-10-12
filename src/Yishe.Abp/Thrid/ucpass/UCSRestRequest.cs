using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Yishe.Abp.Thrid.ucpass
{
    enum EBodyType : uint
    {
        EType_XML = 0,
        EType_JSON
    };

    public class UCSRestRequest
    {
        private string m_restAddress = null;
        private string m_restPort = null;
        private string m_mainAccount = null;
        private string m_mainToken = null;
        private string m_appId = null;
        private bool m_isWriteLog = false;

        private EBodyType m_bodyType = EBodyType.EType_JSON;

        /// <summary>
        /// 服务器api版本
        /// </summary>
        const string softVer = "2014-06-30";

        /// <summary>
        /// 初始化函数
        /// </summary>
        /// <param name="serverIP">服务器地址</param>
        /// <param name="serverPort">服务器端口</param>
        /// <returns></returns>
        public bool init(string restAddress, string restPort)
        {
            this.m_restAddress = restAddress;
            this.m_restPort = restPort;

            if (m_restAddress == null || m_restAddress.Length < 0 || m_restPort == null || m_restPort.Length < 0 || Convert.ToInt32(m_restPort) < 0)
                return false;

            return true;
        }

        /// <summary>
        /// 设置主帐号信息
        /// </summary>
        /// <param name="accountSid">主帐号</param>
        /// <param name="accountToken">主帐号令牌</param>
        public void setAccount(string accountSid, string accountToken)
        {
            this.m_mainAccount = accountSid;
            this.m_mainToken = accountToken;
        }

        /// <summary>
        /// 设置应用ID
        /// </summary>
        /// <param name="appId">应用ID</param>
        public void setAppId(string appId)
        {
            this.m_appId = appId;
        }

        /// <summary>
        /// 日志开关
        /// </summary>
        /// <param name="enable">日志开关</param>
        public void enabeLog(bool enable)
        {
            this.m_isWriteLog = enable;
        }

        /// <summary>
        /// 获取日志路径
        /// </summary>
        /// <returns>日志路径</returns>
        public string GetLogPath()
        {
            string dllpath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            dllpath = dllpath.Substring(8, dllpath.Length - 8);    // 8是 file:// 的长度
            return System.IO.Path.GetDirectoryName(dllpath) + "\\log.txt";
        }

        /// <summary>
        /// 主帐号信息查询
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <returns>包体内容</returns>
        public string QueryAccountInfo()
        {
            try
            {
                string date = DateTime.Now.ToString("yyyyMMddHHmmss");

                // 构建URL内容
                string sigstr = MD5Encrypt(m_mainAccount + m_mainToken + date);
                string uriStr;
                string xml = (m_bodyType == EBodyType.EType_XML ? ".xml" : "");
                uriStr = string.Format("https://{0}:{1}/{2}/Accounts/{3}{4}?sig={5}", m_restAddress, m_restPort, softVer, m_mainAccount, xml, sigstr);
                Uri address = new Uri(uriStr);

                WriteLog("QueryAccountInfo url = " + uriStr);

                // 创建网络请求  
                HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
                setCertificateValidationCallBack();

                // 构建Head
                request.Method = "GET";
                Encoding myEncoding = Encoding.GetEncoding("utf-8");
                byte[] myByte = myEncoding.GetBytes(m_mainAccount + ":" + date);
                string authStr = Convert.ToBase64String(myByte);
                request.Headers.Add("Authorization", authStr);

                if (m_bodyType == EBodyType.EType_XML)
                {
                    request.Accept = "application/xml";
                    request.ContentType = "application/xml;charset=utf-8";
                }
                else
                {
                    request.Accept = "application/json";
                    request.ContentType = "application/json;charset=utf-8";
                }
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string responseStr = reader.ReadToEnd();

                    WriteLog("QueryAccountInfo responseBody = " + responseStr);

                    if (responseStr != null && responseStr.Length > 0)
                    {
                        return responseStr;
                    }
                }
                return null;
                // 获取请求
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        /// <summary>
        /// 申请client帐号
        /// </summary>
        /// <param name="friendlyName">client帐号名称。</param>
        /// <exception cref="ArgumentNullException">参数不能为空</exception>
        /// <exception cref="Exception"></exception>
        /// <returns>包体内容</returns>
        public string CreateClient(string friendlyName, string clientType, string charge, string mobile)
        {

            if (friendlyName == null)
                throw new ArgumentNullException("friendlyName");

            try
            {
                string date = DateTime.Now.ToString("yyyyMMddHHmmss");

                // 构建URL内容
                string sigstr = MD5Encrypt(m_mainAccount + m_mainToken + date);
                string uriStr;
                string xml = (m_bodyType == EBodyType.EType_XML ? ".xml" : "");
                uriStr = string.Format("https://{0}:{1}/{2}/Accounts/{3}/Clients{4}?sig={5}", m_restAddress, m_restPort, softVer, m_mainAccount, xml, sigstr);

                Uri address = new Uri(uriStr);

                WriteLog("CreateClient url = " + uriStr);

                // 创建网络请求  
                HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
                setCertificateValidationCallBack();

                // 构建Head
                request.Method = "POST";

                Encoding myEncoding = Encoding.GetEncoding("utf-8");
                byte[] myByte = myEncoding.GetBytes(m_mainAccount + ":" + date);
                string authStr = Convert.ToBase64String(myByte);
                request.Headers.Add("Authorization", authStr);


                // 构建Body
                StringBuilder data = new StringBuilder();

                if (m_bodyType == EBodyType.EType_XML)
                {
                    request.Accept = "application/xml";
                    request.ContentType = "application/xml;charset=utf-8";

                    data.Append("<?xml version='1.0' encoding='utf-8'?><client>");
                    data.Append("<appId>").Append(m_appId).Append("</appId>");
                    data.Append("<friendlyName>").Append(friendlyName).Append("</friendlyName>");
                    data.Append("<clientType>").Append(clientType).Append("</clientType>");
                    data.Append("<charge>").Append(charge).Append("</charge>");
                    data.Append("<mobile>").Append(mobile).Append("</mobile>");
                    data.Append("</client>");
                }
                else
                {
                    request.Accept = "application/json";
                    request.ContentType = "application/json;charset=utf-8";

                    data.Append("{");
                    data.Append("\"client\":{");
                    data.Append("\"appId\":\"").Append(m_appId).Append("\"");
                    data.Append(",\"friendlyName\":\"").Append(friendlyName).Append("\"");
                    data.Append(",\"clientType\":\"").Append(clientType).Append("\"");
                    data.Append(",\"charge\":\"").Append(charge).Append("\"");
                    data.Append(",\"mobile\":\"").Append(mobile).Append("\"");
                    data.Append("}}");
                }

                byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

                WriteLog("CreateClient requestBody = " + data.ToString());

                // 开始请求
                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.Write(byteData, 0, byteData.Length);
                }

                // 获取请求
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string responseStr = reader.ReadToEnd();

                    WriteLog("CreateClient responseBody = " + responseStr);

                    if (responseStr != null && responseStr.Length > 0)
                    {
                        return responseStr;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 释放client帐号
        /// </summary>
        /// <param name="clientNum">client帐号</param>
        /// <exception cref="ArgumentNullException">参数不能为空</exception>
        /// <exception cref="Exception"></exception>
        /// <returns>包体内容</returns>
        public string DropClient(string clientNum)
        {

            if (clientNum == null)
                throw new ArgumentNullException("clientNum");

            try
            {
                string date = DateTime.Now.ToString("yyyyMMddHHmmss");

                // 构建URL内容
                string sigstr = MD5Encrypt(m_mainAccount + m_mainToken + date);
                string uriStr;
                string xml = (m_bodyType == EBodyType.EType_XML ? ".xml" : "");
                uriStr = string.Format("https://{0}:{1}/{2}/Accounts/{3}/dropClient{4}?sig={5}", m_restAddress, m_restPort, softVer, m_mainAccount, xml, sigstr);

                Uri address = new Uri(uriStr);

                WriteLog("DropClient url = " + uriStr);

                // 创建网络请求  
                HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
                setCertificateValidationCallBack();

                // 构建Head
                request.Method = "POST";

                Encoding myEncoding = Encoding.GetEncoding("utf-8");
                byte[] myByte = myEncoding.GetBytes(m_mainAccount + ":" + date);
                string authStr = Convert.ToBase64String(myByte);
                request.Headers.Add("Authorization", authStr);


                // 构建Body
                StringBuilder data = new StringBuilder();

                if (m_bodyType == EBodyType.EType_XML)
                {
                    request.Accept = "application/xml";
                    request.ContentType = "application/xml;charset=utf-8";

                    data.Append("<?xml version='1.0' encoding='utf-8'?><client>");
                    data.Append("<appId>").Append(m_appId).Append("</appId>");
                    data.Append("<clientNumber>").Append(clientNum).Append("</clientNumber>");
                    data.Append("</client>");
                }
                else
                {
                    request.Accept = "application/json";
                    request.ContentType = "application/json;charset=utf-8";

                    data.Append("{");
                    data.Append("\"client\":{");
                    data.Append("\"appId\":\"").Append(m_appId).Append("\"");
                    data.Append(",\"clientNumber\":\"").Append(clientNum).Append("\"");
                    data.Append("}}");
                }

                byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

                WriteLog("DropClient requestBody = " + data.ToString());

                // 开始请求
                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.Write(byteData, 0, byteData.Length);
                }

                // 获取请求
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string responseStr = reader.ReadToEnd();

                    WriteLog("DropClient responseBody = " + responseStr);

                    if (responseStr != null && responseStr.Length > 0)
                    {
                        return responseStr;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 获取应用下client帐号
        /// </summary>
        /// <param name="startNo">开始的序号，默认从0开始</param>
        /// <param name="offset">一次查询的最大条数，最小是1条，最大是100条</param>
        /// <exception cref="ArgumentOutOfRangeException">参数超出范围</exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public string GetClient(uint startNo, uint offset)
        {

            if (offset < 1 || offset > 100)
            {
                throw new ArgumentOutOfRangeException("offset超出范围");
            }
            try
            {
                string date = DateTime.Now.ToString("yyyyMMddHHmmss");

                // 构建URL内容
                string sigstr = MD5Encrypt(m_mainAccount + m_mainToken + date);
                string uriStr;
                string xml = (m_bodyType == EBodyType.EType_XML ? ".xml" : "");
                uriStr = string.Format("https://{0}:{1}/{2}/Accounts/{3}/clientList{4}?sig={5}", m_restAddress, m_restPort, softVer, m_mainAccount, xml, sigstr);

                Uri address = new Uri(uriStr);

                WriteLog("GetClient url = " + uriStr);

                // 创建网络请求  
                HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
                setCertificateValidationCallBack();

                // 构建Head
                request.Method = "POST";

                Encoding myEncoding = Encoding.GetEncoding("utf-8");
                byte[] myByte = myEncoding.GetBytes(m_mainAccount + ":" + date);
                string authStr = Convert.ToBase64String(myByte);
                request.Headers.Add("Authorization", authStr);


                // 构建Body
                StringBuilder data = new StringBuilder();

                if (m_bodyType == EBodyType.EType_XML)
                {
                    request.Accept = "application/xml";
                    request.ContentType = "application/xml;charset=utf-8";

                    data.Append("<?xml version='1.0' encoding='utf-8'?><client>");
                    data.Append("<appId>").Append(m_appId).Append("</appId>");
                    data.Append("<start>").Append(startNo).Append("</start>");
                    data.Append("<limit>").Append(offset).Append("</limit>");
                    data.Append("</client>");
                }
                else
                {
                    request.Accept = "application/json";
                    request.ContentType = "application/json;charset=utf-8";

                    data.Append("{");
                    data.Append("\"client\":{");
                    data.Append("\"appId\":\"").Append(m_appId).Append("\"");
                    data.Append(",\"start\":\"").Append(startNo).Append("\"");
                    data.Append(",\"clientNumber\":\"").Append(offset).Append("\"");
                    data.Append("}}");
                }

                byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

                WriteLog("GetClient requestBody = " + data.ToString());

                // 开始请求
                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.Write(byteData, 0, byteData.Length);
                }

                // 获取请求
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string responseStr = reader.ReadToEnd();

                    WriteLog("GetClient responseBody = " + responseStr);

                    if (responseStr != null && responseStr.Length > 0)
                    {
                        return responseStr;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// 查询client帐号信息
        /// </summary>
        /// <param name="clientNum">client帐号</param>
        /// <exception cref="ArgumentNullException">参数不能为空</exception>
        /// <exception cref="Exception"></exception>
        /// <returns>包体内容</returns>
        public string QueryClientNumber(string clientNum)
        {
            if (clientNum == null)
                throw new ArgumentNullException("clientNum");

            try
            {
                string date = DateTime.Now.ToString("yyyyMMddHHmmss");

                // 构建URL内容
                string sigstr = MD5Encrypt(m_mainAccount + m_mainToken + date);
                string uriStr;
                string xml = (m_bodyType == EBodyType.EType_XML ? ".xml" : "");
                uriStr = string.Format("https://{0}:{1}/{2}/Accounts/{3}/Clients{4}?sig={5}&clientNumber={6}&appId={7}", m_restAddress, m_restPort, softVer, m_mainAccount, xml, sigstr, clientNum, m_appId);

                Uri address = new Uri(uriStr);

                WriteLog("QueryClientNumber url = " + uriStr);

                // 创建网络请求  
                HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
                setCertificateValidationCallBack();

                // 构建Head
                request.Method = "GET";

                Encoding myEncoding = Encoding.GetEncoding("utf-8");
                byte[] myByte = myEncoding.GetBytes(m_mainAccount + ":" + date);
                string authStr = Convert.ToBase64String(myByte);
                request.Headers.Add("Authorization", authStr);


                // 构建Body
                StringBuilder data = new StringBuilder();

                if (m_bodyType == EBodyType.EType_XML)
                {
                    request.Accept = "application/xml";
                    request.ContentType = "application/xml;charset=utf-8";
                }
                else
                {
                    request.Accept = "application/json";
                    request.ContentType = "application/json;charset=utf-8";
                }
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string responseStr = reader.ReadToEnd();

                    WriteLog("QueryClientNumber responseBody = " + responseStr);

                    if (responseStr != null && responseStr.Length > 0)
                    {
                        return responseStr;
                    }
                }
                return null;

            }
            catch (Exception e)
            {
                throw e;
            }
        }



        /// <summary>
        /// 查询client信息(根据手机号)
        /// </summary>
        /// <param name="clientMobile">client帐号对应的手机号</param>
        /// <exception cref="ArgumentNullException">参数不能为空</exception>
        /// <exception cref="Exception"></exception>
        /// <returns>包体内容</returns>
        public string QueryClientMobile(string clientMobile)
        {
            if (clientMobile == null)
                throw new ArgumentNullException("clientMobile");

            try
            {
                string date = DateTime.Now.ToString("yyyyMMddHHmmss");

                // 构建URL内容
                string sigstr = MD5Encrypt(m_mainAccount + m_mainToken + date);
                string uriStr;
                string xml = (m_bodyType == EBodyType.EType_XML ? ".xml" : "");
                uriStr = string.Format("https://{0}:{1}/{2}/Accounts/{3}/ClientsByMobile{4}?sig={5}&mobile={6}&appId={7}", m_restAddress, m_restPort, softVer, m_mainAccount, xml, sigstr, clientMobile, m_appId);

                Uri address = new Uri(uriStr);

                WriteLog("QueryClient url = " + uriStr);

                // 创建网络请求  
                HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
                setCertificateValidationCallBack();

                // 构建Head
                request.Method = "GET";

                Encoding myEncoding = Encoding.GetEncoding("utf-8");
                byte[] myByte = myEncoding.GetBytes(m_mainAccount + ":" + date);
                string authStr = Convert.ToBase64String(myByte);
                request.Headers.Add("Authorization", authStr);

                StringBuilder data = new StringBuilder();

                if (m_bodyType == EBodyType.EType_XML)
                {
                    request.Accept = "application/xml";
                    request.ContentType = "application/xml;charset=utf-8";
                }
                else
                {
                    request.Accept = "application/json";
                    request.ContentType = "application/json;charset=utf-8";
                }
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string responseStr = reader.ReadToEnd();

                    WriteLog("QueryClientNumber responseBody = " + responseStr);

                    if (responseStr != null && responseStr.Length > 0)
                    {
                        return responseStr;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 应用话单下载
        /// </summary>
        /// <param name="range">day 代表前一天的数据（从00:00 – 23:59）;week代表前一周的数据(周一 到周日)；month表示上一个月的数据（上个月表示当前月减1，如果今天是4月10号，则查询结果是3月份的数据）</param>
        /// <exception cref="ArgumentNullException">参数不能为空</exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public string GetBillList(string range)
        {
            if (range == null)
            {
                throw new ArgumentNullException("range");
            }

            try
            {
                string date = DateTime.Now.ToString("yyyyMMddHHmmss");

                // 构建URL内容
                string sigstr = MD5Encrypt(m_mainAccount + m_mainToken + date);
                string uriStr;
                string xml = (m_bodyType == EBodyType.EType_XML ? ".xml" : "");
                uriStr = string.Format("https://{0}:{1}/{2}/Accounts/{3}/billList{4}?sig={5}", m_restAddress, m_restPort, softVer, m_mainAccount, xml, sigstr);

                Uri address = new Uri(uriStr);

                WriteLog("GetBillList url = " + uriStr);

                // 创建网络请求  
                HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
                setCertificateValidationCallBack();

                // 构建Head
                request.Method = "POST";

                Encoding myEncoding = Encoding.GetEncoding("utf-8");
                byte[] myByte = myEncoding.GetBytes(m_mainAccount + ":" + date);
                string authStr = Convert.ToBase64String(myByte);
                request.Headers.Add("Authorization", authStr);


                // 构建Body
                StringBuilder data = new StringBuilder();

                if (m_bodyType == EBodyType.EType_XML)
                {
                    request.Accept = "application/xml";
                    request.ContentType = "application/xml;charset=utf-8";

                    data.Append("<?xml version='1.0' encoding='utf-8'?><appBill>");
                    data.Append("<appId>").Append(m_appId).Append("</appId>");
                    data.Append("<date>").Append(range).Append("</date>");
                    data.Append("</appBill>");
                }
                else
                {
                    request.Accept = "application/json";
                    request.ContentType = "application/json;charset=utf-8";

                    data.Append("{");
                    data.Append("\"appBill\":{");
                    data.Append("\"appId\":\"").Append(m_appId).Append("\"");
                    data.Append(",\"date\":\"").Append(range).Append("\"");
                    data.Append("}}");
                }

                byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

                WriteLog("CreateSubAccount requestBody = " + data.ToString());

                // 开始请求
                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.Write(byteData, 0, byteData.Length);
                }

                // 获取请求
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string responseStr = reader.ReadToEnd();

                    WriteLog("CreateSubAccount responseBody = " + responseStr);

                    if (responseStr != null && responseStr.Length > 0)
                    {
                        return responseStr;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 话单下载
        /// </summary>
        /// <param name="clientNum"
        /// <param name="range">day 代表前一天的数据（从00:00 – 23:59）;week代表前一周的数据(周一 到周日)；month表示上一个月的数据（上个月表示当前月减1，如果今天是4月10号，则查询结果是3月份的数据）</param>
        /// <exception cref="ArgumentNullException">参数不能为空</exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public string GetClientBillList(string clientNum, string range)
        {
            if (range == null)
            {
                throw new ArgumentNullException("range");
            }
            if (clientNum == null)
            {
                throw new ArgumentNullException("clientNum");
            }
            try
            {
                string date = DateTime.Now.ToString("yyyyMMddHHmmss");

                // 构建URL内容
                string sigstr = MD5Encrypt(m_mainAccount + m_mainToken + date);
                string uriStr;
                string xml = (m_bodyType == EBodyType.EType_XML ? ".xml" : "");
                uriStr = string.Format("https://{0}:{1}/{2}/Accounts/{3}/Clients/billList{4}?sig={5}", m_restAddress, m_restPort, softVer, m_mainAccount, xml, sigstr);

                Uri address = new Uri(uriStr);

                WriteLog("CreateClient url = " + uriStr);

                // 创建网络请求  
                HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
                setCertificateValidationCallBack();

                // 构建Head
                request.Method = "POST";

                Encoding myEncoding = Encoding.GetEncoding("utf-8");
                byte[] myByte = myEncoding.GetBytes(m_mainAccount + ":" + date);
                string authStr = Convert.ToBase64String(myByte);
                request.Headers.Add("Authorization", authStr);


                // 构建Body
                StringBuilder data = new StringBuilder();

                if (m_bodyType == EBodyType.EType_XML)
                {
                    request.Accept = "application/xml";
                    request.ContentType = "application/xml;charset=utf-8";

                    data.Append("<?xml version='1.0' encoding='utf-8'?><clientBill>");
                    data.Append("<appId>").Append(m_appId).Append("</appId>");
                    data.Append("<date>").Append(range).Append("</date>");
                    data.Append("<clientNumber>").Append(clientNum).Append("</clientNumber>");
                    data.Append("</clientBill>");
                }
                else
                {
                    request.Accept = "application/json";
                    request.ContentType = "application/json;charset=utf-8";

                    data.Append("{");
                    data.Append("\"clientBill\":{");
                    data.Append("\"appId\":\"").Append(m_appId).Append("\"");
                    data.Append(",\"date\":\"").Append(range).Append("\"");
                    data.Append(",\"clientNumber\":\"").Append(clientNum).Append("\"");
                    data.Append("}}");
                }

                byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

                WriteLog("CreateSubAccount requestBody = " + data.ToString());

                // 开始请求
                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.Write(byteData, 0, byteData.Length);
                }

                // 获取请求
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string responseStr = reader.ReadToEnd();

                    WriteLog("CreateSubAccount responseBody = " + responseStr);

                    if (responseStr != null && responseStr.Length > 0)
                    {
                        return responseStr;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>
        /// Client充值
        /// </summary>
        /// <param name="clientMobile">client帐号</param>
        /// <param name="chargeType">0 充值；1 回收</param>
        /// <param name="charge">充值或回收的金额</param>
        /// <exception cref="ArgumentNullException">参数不能为空</exception>
        /// <exception cref="Exception"></exception>
        /// <returns>包体内容</returns>
        public string ChargeClient(string clientNum, string chargeType, string charge)
        {

            if (clientNum == null)
                throw new ArgumentNullException("clientNum");
            if (chargeType == null)
                throw new ArgumentNullException("chargeType");
            if (charge == null)
                throw new ArgumentNullException("charge");
            try
            {
                string date = DateTime.Now.ToString("yyyyMMddHHmmss");

                // 构建URL内容
                string sigstr = MD5Encrypt(m_mainAccount + m_mainToken + date);
                string uriStr;
                string xml = (m_bodyType == EBodyType.EType_XML ? ".xml" : "");
                uriStr = string.Format("https://{0}:{1}/{2}/Accounts/{3}/chargeClient{4}?sig={5}", m_restAddress, m_restPort, softVer, m_mainAccount, xml, sigstr);

                Uri address = new Uri(uriStr);

                WriteLog("ChargeClient url = " + uriStr);

                // 创建网络请求  
                HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
                setCertificateValidationCallBack();

                // 构建Head
                request.Method = "POST";

                Encoding myEncoding = Encoding.GetEncoding("utf-8");
                byte[] myByte = myEncoding.GetBytes(m_mainAccount + ":" + date);
                string authStr = Convert.ToBase64String(myByte);
                request.Headers.Add("Authorization", authStr);


                // 构建Body
                StringBuilder data = new StringBuilder();

                if (m_bodyType == EBodyType.EType_XML)
                {
                    request.Accept = "application/xml";
                    request.ContentType = "application/xml;charset=utf-8";

                    data.Append("<?xml version='1.0' encoding='utf-8'?><client>");
                    data.Append("<appId>").Append(m_appId).Append("</appId>");
                    data.Append("<clientNumber>").Append(clientNum).Append("</clientNumber>");
                    data.Append("<chargeType>").Append(chargeType).Append("</chargeType>");
                    data.Append("<charge>").Append(charge).Append("</charge>");
                    data.Append("</client>");
                }
                else
                {
                    request.Accept = "application/json";
                    request.ContentType = "application/json;charset=utf-8";

                    data.Append("{");
                    data.Append("\"client\":{");
                    data.Append("\"appId\":\"").Append(m_appId).Append("\"");
                    data.Append(",\"clientNumber\":\"").Append(clientNum).Append("\"");
                    data.Append(",\"chargeType\":\"").Append(chargeType).Append("\"");
                    data.Append(",\"charge\":\"").Append(charge).Append("\"");
                    data.Append("}}");
                }

                byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

                WriteLog("ChargeClient requestBody = " + data.ToString());

                // 开始请求
                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.Write(byteData, 0, byteData.Length);
                }

                // 获取请求
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string responseStr = reader.ReadToEnd();

                    WriteLog("ChargeClient responseBody = " + responseStr);

                    if (responseStr != null && responseStr.Length > 0)
                    {
                        return responseStr;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }




        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="to">短信接收端手机号码</param>
        /// <param name="templateId">短信模板ID</param>
        /// <param name="param">内容数据，用于替换模板中{数字}</param>
        /// <exception cref="ArgumentNullException">参数不能为空</exception>
        /// <exception cref="Exception"></exception>
        /// <returns>包体内容</returns>
        public string SendSMS(string to, string templateId, string param)
        {

            if (to == null)
            {
                throw new ArgumentNullException("to");
            }

            if (templateId == null)
            {
                throw new ArgumentNullException("templateId");
            }

            try
            {
                string date = DateTime.Now.ToString("yyyyMMddHHmmss");

                // 构建URL内容
                string sigstr = MD5Encrypt(m_mainAccount + m_mainToken + date);
                string uriStr;
                string xml = (m_bodyType == EBodyType.EType_XML ? ".xml" : "");
                uriStr = string.Format("https://{0}:{1}/{2}/Accounts/{3}/Messages/templateSMS{4}?sig={5}", m_restAddress, m_restPort, softVer, m_mainAccount, xml, sigstr);

                Uri address = new Uri(uriStr);

                WriteLog("SendSMS url = " + uriStr);

                // 创建网络请求  
                HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
                setCertificateValidationCallBack();

                // 构建Head
                request.Method = "POST";

                Encoding myEncoding = Encoding.GetEncoding("utf-8");
                byte[] myByte = myEncoding.GetBytes(m_mainAccount + ":" + date);
                string authStr = Convert.ToBase64String(myByte);
                request.Headers.Add("Authorization", authStr);


                // 构建Body
                StringBuilder data = new StringBuilder();

                if (m_bodyType == EBodyType.EType_XML)
                {
                    request.Accept = "application/xml";
                    request.ContentType = "application/xml;charset=utf-8";

                    data.Append("<?xml version='1.0' encoding='utf-8'?><templateSMS>");
                    data.Append("<appId>").Append(m_appId).Append("</appId>");
                    data.Append("<templateId>").Append(templateId).Append("</templateId>");
                    data.Append("<to>").Append(to).Append("</to>");
                    data.Append("<param>").Append(param).Append("</param>");
                    data.Append("</templateSMS>");
                }
                else
                {
                    request.Accept = "application/json";
                    request.ContentType = "application/json;charset=utf-8";

                    data.Append("{");
                    data.Append("\"templateSMS\":{");
                    data.Append("\"appId\":\"").Append(m_appId).Append("\"");
                    data.Append(",\"templateId\":\"").Append(templateId).Append("\"");
                    data.Append(",\"to\":\"").Append(to).Append("\"");
                    data.Append(",\"param\":\"").Append(param).Append("\"");
                    data.Append("}}");
                }

                byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

                WriteLog("CreateSubAccount requestBody = " + data.ToString());

                // 开始请求
                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.Write(byteData, 0, byteData.Length);
                }

                // 获取请求
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string responseStr = reader.ReadToEnd();

                    WriteLog("CreateSubAccount responseBody = " + responseStr);

                    if (responseStr != null && responseStr.Length > 0)
                    {
                        return responseStr;
                    }
                }
                return null;
            }
            catch (Exception e)
            {

                throw e;
            }

        }

        /// <summary>
        /// 双向回呼
        /// </summary>
        /// <param name="fromClient">主叫电话</param>
        /// <param name="toPhone">被叫电话</param>
        /// <param name="fromSerNum">主叫侧显示的号码，只能显示400号码或固话。</param>
        /// <param name="toSerNum">被叫侧显示的号码。可显示手机号码、400号码或固话。</param>
        /// <param name="maxallowtime">被叫侧显示的号码。可显示手机号码、400号码或固话。</param>
        /// <exception cref="ArgumentNullException">参数不能为空</exception>
        /// <exception cref="Exception"></exception>
        /// <returns>包体内容</returns>
        public string CallBack(string fromClient, string toPhone, string fromSerNum, string toSerNum, string maxallowtime)
        {

            if (fromClient == null)
            {
                throw new ArgumentNullException("fromClient");
            }

            if (toPhone == null)
            {
                throw new ArgumentNullException("toPhone");
            }

            try
            {
                string date = DateTime.Now.ToString("yyyyMMddHHmmss");

                // 构建URL内容
                string sigstr = MD5Encrypt(m_mainAccount + m_mainToken + date);
                string uriStr;
                string xml = (m_bodyType == EBodyType.EType_XML ? ".xml" : "");
                uriStr = string.Format("https://{0}:{1}/{2}/Accounts/{3}/Calls/callBack{4}?sig={5}", m_restAddress, m_restPort, softVer, m_mainAccount, xml, sigstr);

                Uri address = new Uri(uriStr);

                WriteLog("CallBack url = " + uriStr);

                // 创建网络请求  
                HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
                setCertificateValidationCallBack();

                // 构建Head
                request.Method = "POST";

                Encoding myEncoding = Encoding.GetEncoding("utf-8");
                byte[] myByte = myEncoding.GetBytes(m_mainAccount + ":" + date);
                string authStr = Convert.ToBase64String(myByte);
                request.Headers.Add("Authorization", authStr);


                // 构建Body
                StringBuilder data = new StringBuilder();

                if (m_bodyType == EBodyType.EType_XML)
                {
                    request.Accept = "application/xml";
                    request.ContentType = "application/xml;charset=utf-8";

                    data.Append("<?xml version='1.0' encoding='utf-8'?><callback>");
                    data.Append("<appId>").Append(m_appId).Append("</appId>");
                    data.Append("<fromClient>").Append(fromClient).Append("</fromClient>");
                    data.Append("<to>").Append(toPhone).Append("</to>");
                    data.Append("<fromSerNum>").Append(fromSerNum).Append("</fromSerNum>");
                    data.Append("<toSerNum>").Append(toSerNum).Append("</toSerNum>");
                    data.Append("<maxallowtime>").Append(maxallowtime).Append("</maxallowtime>");
                    data.Append("</callback>");
                }
                else
                {
                    request.Accept = "application/json";
                    request.ContentType = "application/json;charset=utf-8";

                    data.Append("{");
                    data.Append("\"callback\":{");
                    data.Append("\"appId\":\"").Append(m_appId).Append("\"");
                    data.Append(",\"fromClient\":\"").Append(fromClient).Append("\"");
                    data.Append(",\"to\":\"").Append(toPhone).Append("\"");
                    data.Append(",\"fromSerNum\":\"").Append(fromSerNum).Append("\"");
                    data.Append(",\"toSerNum\":\"").Append(toSerNum).Append("\"");
                    data.Append(",\"maxallowtime\":\"").Append(maxallowtime).Append("\"");
                    data.Append("}}");
                }

                byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

                WriteLog("CreateSubAccount requestBody = " + data.ToString());

                // 开始请求
                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.Write(byteData, 0, byteData.Length);
                }

                // 获取请求
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string responseStr = reader.ReadToEnd();

                    WriteLog("CreateSubAccount responseBody = " + responseStr);

                    if (responseStr != null && responseStr.Length > 0)
                    {
                        return responseStr;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 语音验证码
        /// </summary>
        /// <param name="to">接收号码</param>
        /// <param name="verifyCode">验证码内容，为数字0~9，长度4-8位</param>
        /// <exception cref="ArgumentNullException">参数不能为空</exception>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        public string VoiceCode(string toPhone, string verifyCode)
        {

            if (toPhone == null)
            {
                throw new ArgumentNullException("toPhone");
            }

            if (verifyCode == null)
            {
                throw new ArgumentNullException("verifyCode");
            }

            try
            {
                string date = DateTime.Now.ToString("yyyyMMddHHmmss");

                // 构建URL内容
                string sigstr = MD5Encrypt(m_mainAccount + m_mainToken + date);
                string uriStr;
                string xml = (m_bodyType == EBodyType.EType_XML ? ".xml" : "");
                uriStr = string.Format("https://{0}:{1}/{2}/Accounts/{3}/Calls/voiceCode{4}?sig={5}", m_restAddress, m_restPort, softVer, m_mainAccount, xml, sigstr);

                Uri address = new Uri(uriStr);

                WriteLog("VoiceCode url = " + uriStr);

                // 创建网络请求  
                HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
                setCertificateValidationCallBack();

                // 构建Head
                request.Method = "POST";

                Encoding myEncoding = Encoding.GetEncoding("utf-8");
                byte[] myByte = myEncoding.GetBytes(m_mainAccount + ":" + date);
                string authStr = Convert.ToBase64String(myByte);
                request.Headers.Add("Authorization", authStr);


                // 构建Body
                StringBuilder data = new StringBuilder();

                if (m_bodyType == EBodyType.EType_XML)
                {
                    request.Accept = "application/xml";
                    request.ContentType = "application/xml;charset=utf-8";

                    data.Append("<?xml version='1.0' encoding='utf-8'?><voiceCode>");
                    data.Append("<appId>").Append(m_appId).Append("</appId>");
                    data.Append("<verifyCode>").Append(verifyCode).Append("</verifyCode>");
                    data.Append("<to>").Append(toPhone).Append("</to>");
                    data.Append("</voiceCode>");
                }
                else
                {
                    request.Accept = "application/json";
                    request.ContentType = "application/json;charset=utf-8";

                    data.Append("{");
                    data.Append("\"voiceCode\":{");
                    data.Append("\"appId\":\"").Append(m_appId).Append("\"");
                    data.Append(",\"verifyCode\":\"").Append(verifyCode).Append("\"");
                    data.Append(",\"to\":\"").Append(toPhone).Append("\"");
                    data.Append("}}");
                }

                byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

                WriteLog("CreateSubAccount requestBody = " + data.ToString());

                // 开始请求
                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.Write(byteData, 0, byteData.Length);
                }

                // 获取请求
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    // Get the response stream  
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string responseStr = reader.ReadToEnd();

                    WriteLog("CreateSubAccount responseBody = " + responseStr);

                    if (responseStr != null && responseStr.Length > 0)
                    {
                        return responseStr;
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #region MD5 和 https交互函数定义
        private void WriteLog(string log)
        {
            if (m_isWriteLog)
            {
                string strFilePath = GetLogPath();
                System.IO.FileStream fs = new System.IO.FileStream(strFilePath, System.IO.FileMode.Append);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, System.Text.Encoding.Default);
                sw.WriteLine(DateTime.Now.ToString() + "\t" + log);
                sw.Close();
                fs.Close();
            }
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="source">原内容</param>
        /// <returns>加密后内容</returns>
        public static string MD5Encrypt(string source)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            System.Security.Cryptography.MD5 md5Hasher = System.Security.Cryptography.MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(source));

            // Create a new Stringbuilder to collect the bytes and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("X2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        /// <summary>
        /// 设置服务器证书验证回调
        /// </summary>
        public void setCertificateValidationCallBack()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = CertificateValidationResult;
        }

        /// <summary>
        ///  证书验证回调函数  
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="cer"></param>
        /// <param name="chain"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool CertificateValidationResult(object obj, System.Security.Cryptography.X509Certificates.X509Certificate cer, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors error)
        {
            return true;
        }
        #endregion
    }
}
