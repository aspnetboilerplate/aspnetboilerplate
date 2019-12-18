using System;
using System.Globalization;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Localization;
using AutoMapper;
using System.Linq;

namespace Abp.AutoMapper
{
    public static class AutoMapExtensions
    {
        /// <summary>
        /// Converts an object to another using AutoMapper library. Creates a new object of <typeparamref name="TDestination"/>.
        /// There must be a mapping between objects before calling this method.
        /// </summary>
        /// <typeparam name="TDestination">Type of the destination object</typeparam>
        /// <param name="source">Source object</param>
        [Obsolete("Automapper will remove static API, Please use ObjectMapper instead. See https://github.com/aspnetboilerplate/aspnetboilerplate/issues/4667")]
        public static TDestination MapTo<TDestination>(this object source)
        {
            return AbpEmulateAutoMapper.Mapper != null ? AbpEmulateAutoMapper.Mapper.Map<TDestination>(source) : default(TDestination);
        }

        /// <summary>
        /// Execute a mapping from the source object to the existing destination object
        /// There must be a mapping between objects before calling this method.
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object</param>
        /// <param name="destination">Destination object</param>
        /// <returns></returns>
        [Obsolete("Automapper will remove static API, Please use ObjectMapper instead. See https://github.com/aspnetboilerplate/aspnetboilerplate/issues/4667")]
        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return AbpEmulateAutoMapper.Mapper != null ? AbpEmulateAutoMapper.Mapper.Map(source, destination) : default(TDestination);
        }

        public static CreateMultiLingualMapResult<TMultiLingualEntity, TTranslation, TDestination> CreateMultiLingualMap<TMultiLingualEntity, TMultiLingualEntityPrimaryKey, TTranslation, TDestination>(
            this IMapperConfigurationExpression configuration, MultiLingualMapContext multiLingualMapContext)
            where TTranslation : class, IEntityTranslation<TMultiLingualEntity, TMultiLingualEntityPrimaryKey>
            where TMultiLingualEntity : IMultiLingualEntity<TTranslation>
        {
            var result = new CreateMultiLingualMapResult<TMultiLingualEntity, TTranslation, TDestination>();

            result.TranslationMap = configuration.CreateMap<TTranslation, TDestination>();
            result.EntityMap = configuration.CreateMap<TMultiLingualEntity, TDestination>().BeforeMap((source, destination, context) =>
            {
                var translation = source.Translations.FirstOrDefault(pt => pt.Language == CultureInfo.CurrentUICulture.Name);
                if (translation != null)
                {
                    context.Mapper.Map(translation, destination);
                    return;
                }

                var defaultLanguage = multiLingualMapContext.SettingManager
                                                            .GetSettingValue(LocalizationSettingNames.DefaultLanguage);

                translation = source.Translations.FirstOrDefault(pt => pt.Language == defaultLanguage);
                if (translation != null)
                {
                    context.Mapper.Map(translation, destination);
                    return;
                }

                translation = source.Translations.FirstOrDefault();
                if (translation != null)
                {
                    context.Mapper.Map(translation, destination);
                }
            });

            return result;
        }

        public static CreateMultiLingualMapResult<TMultiLingualEntity, TTranslation, TDestination> CreateMultiLingualMap<TMultiLingualEntity, TTranslation, TDestination>(this IMapperConfigurationExpression configuration, MultiLingualMapContext multiLingualMapContext)
            where TTranslation : class, IEntity, IEntityTranslation<TMultiLingualEntity, int>
            where TMultiLingualEntity : IMultiLingualEntity<TTranslation>
        {
            return configuration.CreateMultiLingualMap<TMultiLingualEntity, int, TTranslation, TDestination>(multiLingualMapContext);
        }
    }
}
