using AutoMapper;

namespace Abp.AutoMapper
{
    public interface IMapTo<TDestination>
    {
    }

    public static class MapExtensions
    {
        public static TDestination MapTo<TDestination>(this IMapTo<TDestination> source)
        {
            return Mapper.Map<TDestination>(source);
        }
    }
}
