using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Abp.EntityFrameworkCore.Extensions;

public static class ModelBuilderExtensions
{
    public static ModelBuilder ConfigureSoftDeleteDbFunction(this ModelBuilder modelBuilder, MethodInfo methodInfo, AbpEfCoreCurrentDbContext abpEfCoreCurrentDbContext)
    {
        modelBuilder.HasDbFunction(methodInfo)
            .HasTranslation(args =>
            {
                // (bool isDeleted, bool boolParam)
                var isDeleted = args[0];
                var boolParam = args[1];

                if (abpEfCoreCurrentDbContext.GetRequiredContext().IsSoftDeleteFilterEnabled)
                {
                    // IsDeleted == false. Use the column's TypeMapping so any ValueConverter is applied to the literal.
                    return new SqlBinaryExpression(
                        ExpressionType.Equal,
                        isDeleted,
                        new SqlConstantExpression(false, typeof(bool), isDeleted.TypeMapping ?? boolParam.TypeMapping),
                        boolParam.Type,
                        boolParam.TypeMapping);
                }

                // empty where sql
                return new SqlConstantExpression(true, boolParam.Type, boolParam.TypeMapping);
            });

        return modelBuilder;
    }

    public static ModelBuilder ConfigureMayHaveTenantDbFunction(this ModelBuilder modelBuilder, MethodInfo methodInfo, AbpEfCoreCurrentDbContext abpEfCoreCurrentDbContext)
    {
        modelBuilder.HasDbFunction(methodInfo)
            .HasTranslation(args =>
            {
                // (int? tenantId, int? currentTenantId, bool boolParam)
                var tenantId = args[0];
                var currentTenantId = args[1];
                var boolParam = args[2];

                if (abpEfCoreCurrentDbContext.GetRequiredContext().IsMayHaveTenantFilterEnabled)
                {
                    // TenantId == CurrentTenantId
                    return new SqlBinaryExpression(
                        ExpressionType.Equal,
                        tenantId,
                        currentTenantId,
                        boolParam.Type,
                        boolParam.TypeMapping);
                }

                // empty where sql
                return new SqlConstantExpression(true, boolParam.Type, boolParam.TypeMapping);
            });

        return modelBuilder;
    }

    public static ModelBuilder ConfigureMustHaveTenantDbFunction(this ModelBuilder modelBuilder, MethodInfo methodInfo, AbpEfCoreCurrentDbContext abpEfCoreCurrentDbContext)
    {
        modelBuilder.HasDbFunction(methodInfo)
            .HasTranslation(args =>
            {
                // (int tenantId, int? currentTenantId, bool boolParam)
                var tenantId = args[0];
                var currentTenantId = args[1];
                var boolParam = args[2];

                if (abpEfCoreCurrentDbContext.GetRequiredContext().IsMustHaveTenantFilterEnabled)
                {
                    // TenantId == CurrentTenantId
                    return new SqlBinaryExpression(
                        ExpressionType.Equal,
                        tenantId,
                        currentTenantId,
                        boolParam.Type,
                        boolParam.TypeMapping);
                }

                // empty where sql
                return new SqlConstantExpression(true, boolParam.Type, boolParam.TypeMapping);
            });

        return modelBuilder;
    }
}
