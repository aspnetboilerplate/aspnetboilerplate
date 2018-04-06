using System;
using System.Linq.Expressions;
using System.Reflection;
using Abp.Reflection;
using Abp.Timing;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Abp.EntityFrameworkCore
{
    // todo: We can remove this class and use ValueConverters of EF Core when EF 2.1 is released 
    // if ValueConverters can handle DateTime kind normalization properly
    // See https://github.com/aspnet/EntityFrameworkCore/issues/242
    public class AbpEntityMaterializerSource : EntityMaterializerSource
    {
        private static readonly MethodInfo NormalizeDateTimeMethod =
            typeof(AbpEntityMaterializerSource).GetTypeInfo().GetMethod(nameof(NormalizeDateTime), BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly MethodInfo NormalizeNullableDateTimeMethod =
            typeof(AbpEntityMaterializerSource).GetTypeInfo().GetMethod(nameof(NormalizeNullableDateTime), BindingFlags.Static | BindingFlags.NonPublic);

        public override Expression CreateReadValueExpression(Expression valueBuffer, Type type, int index, IProperty property = null)
        {
            if (ShouldDisableDateTimeNormalization(property))
            {
                return base.CreateReadValueExpression(valueBuffer, type, index, property);
            }

            if (type == typeof(DateTime))
            {
                return Expression.Call(
                    NormalizeDateTimeMethod,
                    base.CreateReadValueExpression(valueBuffer, type, index, property)
                );
            }

            if (type == typeof(DateTime?))
            {
                return Expression.Call(
                    NormalizeNullableDateTimeMethod,
                    base.CreateReadValueExpression(valueBuffer, type, index, property)
                );
            }

            return base.CreateReadValueExpression(valueBuffer, type, index, property);
        }

        private static DateTime NormalizeDateTime(DateTime value)
        {
            return Clock.Normalize(value);
        }

        private static DateTime? NormalizeNullableDateTime(DateTime? value)
        {
            if (value == null)
            {
                return null;
            }

            return Clock.Normalize(value.Value);
        }

        private static bool ShouldDisableDateTimeNormalization(IProperty property)
        {
            if (property == null)
            {
                return false;
            }

            if (property.PropertyInfo.IsDefined(typeof(DisableDateTimeNormalizationAttribute), true))
            {
                return true;
            }

            property.TryGetMemberInfo(false, false, out var memberInfo, out _);
            return ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<DisableDateTimeNormalizationAttribute>(memberInfo) != null;
        }
    }
}