using System;

namespace Abp.EntityFramework.GraphDiff.Mapping
{
    public class EntityMapping
    {
        public Type EntityType;

        public object MappingExpression { get; set; }

        public EntityMapping(Type type, object mappingExpression)
        {
            EntityType = type;
            MappingExpression = mappingExpression;
        }
    }
}