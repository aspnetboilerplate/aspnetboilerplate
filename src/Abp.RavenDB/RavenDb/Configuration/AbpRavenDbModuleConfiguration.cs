using System;

namespace Abp.RavenDb.Configuration
{
    internal class AbpRavenDbModuleConfiguration : IAbpRavenDbModuleConfiguration
    {
        public string Url { get; set; }

        public string DatabaseName { get; set; }

        public bool Equals(IAbpRavenDbModuleConfiguration other)
        {
            if (other == null)
                return false;

            return string.Equals(Url, other.Url, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(DatabaseName, other.DatabaseName, StringComparison.OrdinalIgnoreCase);
        }
    }
}