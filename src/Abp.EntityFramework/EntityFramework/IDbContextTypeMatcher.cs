using System;

namespace Abp.EntityFramework
{
    public interface IDbContextTypeMatcher
    {
        void Add(Type sourceType, Type targetType);
        Type GetConcreteType(Type dbContextType);
    }
}