using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq.Expressions;

namespace Abp.Zero.EntityFramework
{
    public class IndexingPropertyConvention<T, TProperty> : Convention where T : class
    {
        public IndexingPropertyConvention(Expression<Func<T, TProperty>> propertyExpression, Func<Type, bool> predicate = null)
        {
            predicate = predicate ?? (t => true);
            Types<T>()
                .Where(t => predicate(t))
                .Configure(e => e.Property(propertyExpression).HasColumnAnnotation(IndexAnnotation.AnnotationName, new IndexAnnotation(new IndexAttribute
                {
                    IsClustered = false,
                    IsUnique = false
                })));
        }
    }
}