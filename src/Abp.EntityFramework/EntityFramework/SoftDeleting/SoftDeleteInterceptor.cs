using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Interception;

namespace Abp.EntityFramework.SoftDeleting
{
    internal class SoftDeleteInterceptor : IDbCommandTreeInterceptor
    {
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
            }
        }
    }
}
