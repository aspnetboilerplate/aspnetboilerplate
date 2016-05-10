using System.Collections.Generic;
using Abp.MultiTenancy;

namespace Abp.Domain.Uow
{
    public class ConnectionStringResolveArgs : Dictionary<string, object>
    {
        public ConnectionStringResolveArgs(MultiTenancySides? multiTenancySide = null)
        {
            MultiTenancySide = multiTenancySide;
        }

        public MultiTenancySides? MultiTenancySide { get; set; }
    }
}