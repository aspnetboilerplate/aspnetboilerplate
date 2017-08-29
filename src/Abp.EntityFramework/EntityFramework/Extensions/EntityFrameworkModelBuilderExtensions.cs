using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace Abp.EntityFramework.Extensions
{
    //TODO: We can create simpler extension methods to create indexes
    //TODO: Check https://github.com/mj1856/EntityFramework.IndexingExtensions for example
    public static class EntityFrameworkModelBuilderExtensions
    {
        public static PrimitivePropertyConfiguration CreateIndex(this PrimitivePropertyConfiguration propertyConfiguration)
        {
            return propertyConfiguration.HasColumnAnnotation(
                IndexAnnotation.AnnotationName,
                new IndexAnnotation(
                    new IndexAttribute()
                    )
                );
        }

        public static PrimitivePropertyConfiguration CreateIndex(this PrimitivePropertyConfiguration propertyConfiguration, string name)
        {
            return propertyConfiguration.HasColumnAnnotation(
                IndexAnnotation.AnnotationName,
                new IndexAnnotation(
                    new IndexAttribute(name)
                    )
                );
        }

        public static PrimitivePropertyConfiguration CreateIndex(this PrimitivePropertyConfiguration propertyConfiguration, string name, int order)
        {
            return propertyConfiguration.HasColumnAnnotation(
                IndexAnnotation.AnnotationName,
                new IndexAnnotation(
                    new IndexAttribute(name, order)
                    )
                );
        }
    }
}