using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace Abp.EntityFramework.SoftDeleting
{
    internal class SoftDeleteQueryVisitor : DefaultExpressionVisitor
    {
        public override DbExpression Visit(DbScanExpression expression)
        {
            MetadataProperty annotation = expression.Target.ElementType.MetadataProperties
                                .SingleOrDefault(p => p.Name.EndsWith("customannotation:" + AbpEfConsts.SoftDeleteCustomAnnotationName));

            if (annotation != null)
            {
                // Tests if we use the attribute name property, if not we use default property of ISoftDelete Interface
                string column = annotation.Value is bool ? "IsDeleted" : annotation.Value.ToString();


                var table = (EntityType)expression.Target.ElementType;

                var prop = table.Properties.Any(p => p.Name == column) ? column : "IsDeleted";

                var binding = expression.Bind();
                return binding.Filter(binding.VariableType
                    .Variable(binding.VariableName)
                    .Property(prop)
                    .NotEqual(DbExpression.FromBoolean(true)));

            }

            return base.Visit(expression);
        }
    }
}