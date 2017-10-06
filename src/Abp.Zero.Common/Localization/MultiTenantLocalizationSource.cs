using System;
using System.Globalization;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Localization.Dictionaries;
using Castle.Core.Logging;

namespace Abp.Localization
{
    public class MultiTenantLocalizationSource : DictionaryBasedLocalizationSource, IMultiTenantLocalizationSource
    {
        public new MultiTenantLocalizationDictionaryProvider DictionaryProvider
        {
            get { return base.DictionaryProvider.As<MultiTenantLocalizationDictionaryProvider>(); }
        }

        public ILogger Logger { get; set; }

        public MultiTenantLocalizationSource(string name, MultiTenantLocalizationDictionaryProvider dictionaryProvider) 
            : base(name, dictionaryProvider)
        {
            Logger = NullLogger.Instance;
        }

        public override void Initialize(ILocalizationConfiguration configuration, IIocResolver iocResolver)
        {
            base.Initialize(configuration, iocResolver);

            if (Logger is NullLogger && iocResolver.IsRegistered(typeof(ILoggerFactory)))
            {
                Logger = iocResolver.Resolve<ILoggerFactory>().Create(typeof (MultiTenantLocalizationSource));
            }
        }

        public string GetString(int? tenantId, string name, CultureInfo culture)
        {
            var value = GetStringOrNull(tenantId, name, culture);

            if (value == null)
            {
                return ReturnGivenNameOrThrowException(name, culture);
            }

            return value;
        }

        public string GetStringOrNull(int? tenantId, string name, CultureInfo culture, bool tryDefaults = true)
        {
            var cultureName = culture.Name;
            var dictionaries = DictionaryProvider.Dictionaries;

            //Try to get from original dictionary (with country code)
            ILocalizationDictionary originalDictionary;
            if (dictionaries.TryGetValue(cultureName, out originalDictionary))
            {
                var strOriginal = originalDictionary
                    .As<IMultiTenantLocalizationDictionary>()
                    .GetOrNull(tenantId, name);

                if (strOriginal != null)
                {
                    return strOriginal.Value;
                }
            }

            if (!tryDefaults)
            {
                return null;
            }

            //Try to get from same language dictionary (without country code)
            if (cultureName.Contains("-")) //Example: "tr-TR" (length=5)
            {
                ILocalizationDictionary langDictionary;
                if (dictionaries.TryGetValue(GetBaseCultureName(cultureName), out langDictionary))
                {
                    var strLang = langDictionary.As<IMultiTenantLocalizationDictionary>().GetOrNull(tenantId, name);
                    if (strLang != null)
                    {
                        return strLang.Value;
                    }
                }
            }

            //Try to get from default language
            var defaultDictionary = DictionaryProvider.DefaultDictionary;
            if (defaultDictionary == null)
            {
                return null;
            }

            var strDefault = defaultDictionary.As<IMultiTenantLocalizationDictionary>().GetOrNull(tenantId, name);
            if (strDefault == null)
            {
                return null;
            }

            return strDefault.Value;
        }

        public override void Extend(ILocalizationDictionary dictionary)
        {
            DictionaryProvider.Extend(dictionary);
        }

        private static string GetBaseCultureName(string cultureName)
        {
            return cultureName.Contains("-")
                ? cultureName.Left(cultureName.IndexOf("-", StringComparison.Ordinal))
                : cultureName;
        }
    }
}