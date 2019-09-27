using System;
using System.Linq.Expressions;
using Abp.Timing;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Abp.EntityFrameworkCore.ValueConverters
{
    public class AbpDateTimeConverter : ValueConverter<DateTime?, DateTime?>
    {
        public AbpDateTimeConverter([CanBeNull] ConverterMappingHints mappingHints = default)
            : base(Normalize, Normalize, mappingHints)
        {
        }

        private static readonly Expression<Func<DateTime?, DateTime?>> Normalize = x =>
            x.HasValue ? Clock.Normalize(x.Value) : x;
    }
}