using System;

namespace Abp.RavenDb.Configuration
{
    public interface IAbpRavenDbModuleConfiguration : IEquatable<IAbpRavenDbModuleConfiguration>
    {
        string Url { get; set; }

        string DatabaseName { get; set; }
    }
}
