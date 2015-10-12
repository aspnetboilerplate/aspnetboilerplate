using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Yishe.Abp.Thrid.ucpass
{
    class CreateToken
    {
        public string createToken(string mainAccount, string mianToken, string client, string clientPwd, string expireTime = null)
        {
            if (expireTime == null)
            {
                expireTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            string head;
            string body;

            StringBuilder headData = new StringBuilder();
            StringBuilder bodyData = new StringBuilder();

            headData.Append("{");
            headData.Append("\"Alg\":\"HS256\",\"Accid\":\"").Append(mainAccount).Append("\"");
            headData.Append(",\"Cnumber\":\"").Append(client).Append("\"");
            headData.Append(",\"Expiretime\":\"").Append(expireTime).Append("\"");
            headData.Append("}");

            bodyData.Append("{");
            bodyData.Append("\"Accid\":\"").Append(mainAccount).Append("\"");
            bodyData.Append(",\"AccToken\":\"").Append(mianToken).Append("\"");
            bodyData.Append(",\"Cnumber\":\"").Append(client).Append("\"");
            bodyData.Append(",\"Cpwd\":\"").Append(clientPwd).Append("\"");
            bodyData.Append(",\"Expiretime\":\"").Append(expireTime).Append("\"");
            bodyData.Append("}");

            head = headData.ToString();
            body = bodyData.ToString();

            Encoding myEncoding = Encoding.GetEncoding("utf-8");

            byte[] SHA256Byte = myEncoding.GetBytes(body);
            HMACSHA256 sha256 = new HMACSHA256(myEncoding.GetBytes(mianToken));
            byte[] bodyByte = sha256.ComputeHash(SHA256Byte);
            body = Convert.ToBase64String(bodyByte);

            byte[] headByte = myEncoding.GetBytes(head);
            head = Convert.ToBase64String(headByte);

            string token = head + "." + body;
            return token;
        }
    }
}
