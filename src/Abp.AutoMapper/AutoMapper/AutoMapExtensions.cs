
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
        public static TDestination MapTo<TDestination>(this object source)
        {
            return Mapper.Map<TDestination>(source);
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
        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return Mapper.Map(source, destination);
        }

        public static void CreateMultiLingualMap<TMultiLingualEntity, TTranslation, TDestination>(this IMapperConfigurationExpression configuration, IIocResolver iocResolver)
            where TTranslation : class, IEntity, IEntityTranslation
            where TMultiLingualEntity : IMultiLingualEntity<TTranslation>
        {
            configuration.CreateMap<TMultiLingualEntity, TDestination>().AfterMap((source, destination, context) =>
            {
                var settingManager = iocResolver.Resolve<ISettingManager>();
                var currentLanguage = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
                var defaultLanguage = settingManager.GetSettingValue(LocalizationSettingNames.DefaultLanguage);

                if (source.Translations.Any(t => t.Language == currentLanguage))
                {
                    var currentTranlation = source.Translations.Single(pt => pt.Language == currentLanguage);
                    context.Mapper.Map(currentTranlation, destination);
                }
                else if (source.Translations.Any(t => t.Language == defaultLanguage))
                {
                    var defaultTranlation = source.Translations.Single(pt => pt.Language == defaultLanguage);
                    context.Mapper.Map(defaultTranlation, destination);
                }
                else if (source.Translations.Any())
                {
                    var tranlation = source.Translations.First();
                    context.Mapper.Map(tranlation, destination);
                }
            });
        }
    }
}
