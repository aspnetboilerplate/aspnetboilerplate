using System;

namespace Abp.EntityFrameworkCore
{
    public interface IDbContextTypeMatcher
    {
        void Add(Type sourceDbContextType, Type targetDbContextType);

        void Populate(Type[] dbContextTypes);

        Type GetConcreteType(Type sourceDbContextType);
    }
}