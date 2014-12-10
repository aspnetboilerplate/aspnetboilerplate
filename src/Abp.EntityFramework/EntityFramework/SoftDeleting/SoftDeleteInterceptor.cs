using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;

namespace Abp.EntityFramework.SoftDeleting
{
    /// <summary>
    ///     A soft delete interceptor.
    /// </summary>
    /// <seealso cref="T:System.Data.Entity.Infrastructure.Interception.IDbCommandTreeInterceptor"/>
    internal class SoftDeleteInterceptor : IDbCommandTreeInterceptor
    {
        /// <summary>
        ///     This method is called after a new
        ///     <see cref="T:System.Data.Entity.Core.Common.CommandTrees.DbCommandTree"/> has been
        ///     created. The tree that is used after interception can be changed by setting
        ///     <see cref="P:System.Data.Entity.Infrastructure.Interception.DbCommandTreeInterceptionContext.Result"/>
        ///     while intercepting.
        /// </summary>
        /// <param name="interceptionContext">Contextual information associated with the call.</param>
        /// <seealso cref="M:System.Data.Entity.Infrastructure.Interception.IDbCommandTreeInterceptor.TreeCreated(DbCommandTreeInterceptionContext)"/>
        public void TreeCreated(DbCommandTreeInterceptionContext interceptionContext)
        {
            if (interceptionContext.OriginalResult.DataSpace == DataSpace.SSpace)
            {
                var queryCommand = interceptionContext.Result as DbQueryCommandTree;
                if (queryCommand != null)
                {
                    var newQuery = queryCommand.Query.Accept(new SoftDeleteQueryVisitor());
                    interceptionContext.Result = new DbQueryCommandTree(queryCommand.MetadataWorkspace, queryCommand.DataSpace, newQuery);
                }

                var deleteCommand = interceptionContext.OriginalResult as DbDeleteCommandTree;
                if (deleteCommand != null)
                {
                    MetadataProperty annotation = deleteCommand.Target.VariableType.EdmType.MetadataProperties
                                .SingleOrDefault(p => p.Name.EndsWith("customannotation:" + AbpEfConsts.SoftDeleteCustomAnnotationName));

                    if (annotation != null)
                    {
                        string column = annotation.Value is bool ? "IsDeleted" : annotation.Value.ToString();

                        var setClauses = new List<DbModificationClause>();
                        var table = (EntityType)deleteCommand.Target.VariableType.EdmType;
                        if (table.Properties.Any(p => p.Name == column))
                        {
                            setClauses.Add(DbExpressionBuilder.SetClause(
                                    deleteCommand.Target.VariableType.Variable(deleteCommand.Target.VariableName).Property(column),
                                    DbExpression.FromBoolean(true)));
                        }

                        var update = new DbUpdateCommandTree(
                            deleteCommand.MetadataWorkspace,
                            deleteCommand.DataSpace,
                            deleteCommand.Target,
                            deleteCommand.Predicate,
                            setClauses.AsReadOnly(),
                            null);

                        interceptionContext.Result = update;
                    }
                }
            }
        }
    }
}
