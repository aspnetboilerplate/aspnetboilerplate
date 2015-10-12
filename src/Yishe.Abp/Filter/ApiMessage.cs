using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Yishe.Abp
{
    public enum Code
    {

        /// <summary>
        /// 请求成功
        /// </summary>
        Ok = 0,
        VaildCodeError = 4001,
        VaildCodeExpire = 4002,
        VaildCodeTooMany = 4003,
        VaildCodeHasSent = 4004,
        PhoneHasExist = 4005,
        SimplePasswd = 4006,
        RequestTooMany = 4007,
        InputFormatError = 4008,
        UserIsLock = 4009,
        RequestTokenInvaild = 4010,
        UserOrPasswordError=4011,
        Error = 5000




    }

    public interface IApiResult<T>
    {
        int Code { get; set; }
        string Msg { get; set; }

        T Result { get; set; }
    }

    [Serializable]
    [DataContract]
    public class ApiResult<T> : IApiResult<T>
    {
        public ApiResult()
        {

        }


        public ApiResult(Code msgType, string errorMsg)
        {
            this.Code = (int)msgType;
            this.Msg = errorMsg;
        }

        public ApiResult(Code msgType, string logMsg, T result)
        {
            this.Code = (int)msgType;
            this.Msg = logMsg;
            this.Result = result;
        }

        [DataMember]
        public int Code { get; set; }
        [DataMember]
        public string Msg { get; set; }

        [DataMember]
        public T Result { get; set; }
    }

    [Serializable]
    [DataContract]
    public class ApiResult: ApiResult<object>
    {
    }
}
