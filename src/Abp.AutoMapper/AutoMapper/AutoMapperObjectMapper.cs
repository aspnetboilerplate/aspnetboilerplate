using System.Linq;
using AutoMapper;
using IObjectMapper = Abp.ObjectMapping.IObjectMapper;

namespace Abp.AutoMapper
{
    public class AutoMapperObjectMapper : IObjectMapper
    {
        protected readonly IMapper Mapper;

        public AutoMapperObjectMapper(IMapper mapper)
        {
            Mapper = mapper;
        }

        public TDestination Map<TDestination>(object source)
        {
            return Mapper.Map<TDestination>(source);
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return Mapper.Map(source, destination);
        }

        public IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source)
        {
            return Mapper.ProjectTo<TDestination>(source);
        }
    }
}
