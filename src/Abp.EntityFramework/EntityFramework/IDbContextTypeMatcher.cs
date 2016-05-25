using System;

namespace Abp.EntityFramework
{
    public interface IDbContextTypeMatcher
    {
        void Add(Type sourceDbContextType, Type targetDbContextType);
        Type GetConcreteType(Type sourceDbContextType);
    }
}