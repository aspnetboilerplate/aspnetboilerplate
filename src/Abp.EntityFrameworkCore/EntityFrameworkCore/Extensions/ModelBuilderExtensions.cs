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

                if (GetCurrentDbContext(abpEfCoreCurrentDbContext).IsSoftDeleteFilterEnabled)
                {
                    // IsDeleted == false
                    return new SqlBinaryExpression(
                        ExpressionType.Equal,
                        isDeleted,
                        new SqlConstantExpression(false, boolParam.TypeMapping),
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

                if (GetCurrentDbContext(abpEfCoreCurrentDbContext).IsMayHaveTenantFilterEnabled)
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

                if (GetCurrentDbContext(abpEfCoreCurrentDbContext).IsMustHaveTenantFilterEnabled)
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

    // The filter state is read while the query is translated. AbpEntityQueryProvider sets the current
    // DbContext for that, but some queries are compiled without it (EF.CompileQuery, EF.CompileAsyncQuery
    // and third-party libraries such as EF Plus batches and bulk operations). Such queries must fail
    // instead of running without the data filters.
    private static AbpDbContext GetCurrentDbContext(AbpEfCoreCurrentDbContext abpEfCoreCurrentDbContext)
    {
        var context = abpEfCoreCurrentDbContext.Context;
        if (context == null)
        {
            throw new AbpException(
                "ABP cannot apply the data filters to a query that is compiled outside of AbpEntityQueryProvider " +
                "(for example EF.CompileQuery, EF.CompileAsyncQuery, or batch/bulk operations of third-party libraries). " +
                "UseAbpQueryCompiler does not support this kind of query. " +
                "Run the query as a normal LINQ query or set 'UseAbpQueryCompiler' of 'IAbpEfCoreConfiguration' to false.");
        }

        return context;
    }
}
