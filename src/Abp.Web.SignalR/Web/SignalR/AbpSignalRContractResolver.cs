using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNet.SignalR.Infrastructure;
using Newtonsoft.Json.Serialization;

namespace Abp.Web.SignalR
{
    /// <summary>
    /// 使用CamelCasePropertyNamesContractResolver代替DefaultContractResolver进行SignalR通信。
    /// </summary>
    public class AbpSignalRContractResolver : IContractResolver
    {
        /// <summary>
        /// 忽略的程序集列表。
        /// 它只包含SignalR自己的程序集。
        /// 如果您不想让您的程序集的类型在发送给客户端时自动将其套用，则将其添加到此列表中。
        /// </summary>
        public static List<Assembly> IgnoredAssemblies { get; private set; }

        private readonly IContractResolver _camelCaseContractResolver;
        private readonly IContractResolver _defaultContractSerializer;

        static AbpSignalRContractResolver()
        {
            IgnoredAssemblies = new List<Assembly>
            {
                typeof (Connection).Assembly
            };
        }

        /// <summary>
        /// 初始化<see cref ="AbpSignalRContractResolver"/>类的新实例。
        /// </summary>
        public AbpSignalRContractResolver()
        {
            _defaultContractSerializer = new DefaultContractResolver();
            _camelCaseContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        public JsonContract ResolveContract(Type type)
        {
            if (IgnoredAssemblies.Contains(type.Assembly))
            {
                return _defaultContractSerializer.ResolveContract(type);
            }

            return _camelCaseContractResolver.ResolveContract(type);
        }
    }
}
