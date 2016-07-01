using System;

namespace Abp
{
    public interface IAbpStartupModuleAccessor
    {
        Type StartupModule { get; }
    }
}