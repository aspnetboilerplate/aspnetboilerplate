using AutoMapper;

namespace Abp.AutoMapper
{
    public class CreateMultiLingualMapResult<TMultiLingualEntity, TTranslation, TDestination>
    {
        public IMappingExpression<TTranslation, TDestination> TranslationMap { get; set; }

        public IMappingExpression<TMultiLingualEntity, TDestination> EntityMap { get; set; }
    }
}