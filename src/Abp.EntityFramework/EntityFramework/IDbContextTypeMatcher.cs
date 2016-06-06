using System;
using System.Collections.Generic;

namespace Abp.EntityFramework
{
    public interface IDbContextTypeMatcher
    {
        void Add(Type sourceDbContextType, Type targetDbContextType);

        void Populate(Type[] dbContextTypes);

        Type GetConcreteType(Type sourceDbContextType);
    }
}