using System.Linq;
using System.Reflection;
using Abp.MqMessages.Handlers;

namespace Abp.MqMessages.Mapper
{
    /// <summary>
    /// AutoMapper helper for <see cref="EventDataPublishHandlerBase<,>"/>, map EventData into MqMessage
    /// </summary>
    public static class AutoEventsMapToMqMessagesHelper
    {
        public static void CreateEventsToMqMessagesMappings(this global::AutoMapper.IMapperConfigurationExpression mapper, Assembly assembly)
        {
            var typesToRegister = assembly.GetTypes()
                .Where(type => !string.IsNullOrEmpty(type.Namespace))
                .Where(type => type.GetTypeInfo().BaseType != null
                && type.GetTypeInfo().BaseType.GetTypeInfo().IsGenericType
                && (type.GetTypeInfo().BaseType.GetGenericTypeDefinition() == typeof(EventDataPublishHandlerBase<,>)));

            foreach (var type in typesToRegister)
            {
                var genericArgs = type.GetTypeInfo().BaseType.GetGenericArguments();
                if (genericArgs.Length > 1)
                {
                    mapper.CreateMap(genericArgs[0], genericArgs[1]);
                }
            }
        }
    }
}
