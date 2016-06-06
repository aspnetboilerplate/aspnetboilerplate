using Abp.Dependency;
using Abp.ObjectMapping;

namespace Abp.AutoMapper
{
    public class AutoMapperObjectMapper : IObjectMapper, ISingletonDependency
    {
        public TDestination Map<TDestination>(object source)
        {
            return source.MapTo<TDestination>();
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return source.MapTo(destination);
        }
    }
}
