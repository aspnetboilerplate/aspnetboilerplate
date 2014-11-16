using AutoMapper;

namespace Abp.AutoMapper
{
    public static class AutoMapExtensions
    {
        public static TDestination Map<TDestination>(this IAutoMapTo<TDestination> source)
        {
            return Mapper.Map<TDestination>(source);
        }

        public static TDestination Map<TSource, TDestination>(this TSource source)
            where TDestination : IAutoMapFrom<TSource>
        {
            return Mapper.Map<TDestination>(source);
        }
    }
}
