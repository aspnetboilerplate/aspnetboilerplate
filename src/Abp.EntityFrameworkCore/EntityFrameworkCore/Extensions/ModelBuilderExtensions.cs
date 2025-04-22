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
                var isDeleted = args[0];
                var boolParam = args[1];

                if (abpEfCoreCurrentDbContext.Context?.IsSoftDeleteFilterEnabled == true)
                {
                    // IsDeleted == false
                    return new SqlBinaryExpression(
                        ExpressionType.Equal,
                        isDeleted,
                        new SqlConstantExpression(false, isDeleted.TypeMapping),
                        isDeleted.Type,
                        isDeleted.TypeMapping);
                }

                // empty where sql
                return new SqlBinaryExpression(
                    ExpressionType.Equal,
                    new SqlConstantExpression(true, boolParam.TypeMapping),
                    new SqlConstantExpression(true, boolParam.TypeMapping),
                    boolParam.Type,
                    boolParam.TypeMapping);
            });

        return modelBuilder;
    }

    public static ModelBuilder ConfigureMayHaveTenantDbFunction(this ModelBuilder modelBuilder, MethodInfo methodInfo, AbpEfCoreCurrentDbContext abpEfCoreCurrentDbContext)
    {
        modelBuilder.HasDbFunction(methodInfo)
            .HasTranslation(args =>
            {
                var tenantId = args[0];
                var currentTenantId = args[1];
                var boolParam = args[2];

                if (abpEfCoreCurrentDbContext.Context?.IsMayHaveTenantFilterEnabled == true)
                {
                    return new SqlBinaryExpression(
                        ExpressionType.Equal,
                        tenantId,
                        currentTenantId,
                        tenantId.Type,
                        tenantId.TypeMapping);
                }

                return new SqlBinaryExpression(
                    ExpressionType.Equal,
                    new SqlConstantExpression(true, boolParam.TypeMapping),
                    new SqlConstantExpression(true, boolParam.TypeMapping),
                    boolParam.Type,
                    boolParam.TypeMapping);
            });

        return modelBuilder;
    }

    public static ModelBuilder ConfigureMustHaveTenantDbFunction(this ModelBuilder modelBuilder, MethodInfo methodInfo, AbpEfCoreCurrentDbContext abpEfCoreCurrentDbContext)
    {
        modelBuilder.HasDbFunction(methodInfo)
            .HasTranslation(args =>
            {
                var tenantId = args[0];
                var currentTenantId = args[1];
                var boolParam = args[2];

                if (abpEfCoreCurrentDbContext.Context?.IsMustHaveTenantFilterEnabled == true)
                {
                    return new SqlBinaryExpression(
                        ExpressionType.Equal,
                        tenantId,
                        currentTenantId,
                        tenantId.Type,
                        tenantId.TypeMapping);
                }

                return new SqlBinaryExpression(
                    ExpressionType.Equal,
                    new SqlConstantExpression(true, boolParam.TypeMapping),
                    new SqlConstantExpression(true, boolParam.TypeMapping),
                    boolParam.Type,
                    boolParam.TypeMapping);
            });

        return modelBuilder;
    }
}