using System;

namespace Abp.RavenDb.Configuration
{
    internal class AbpRavenDbModuleConfiguration : IAbpRavenDbModuleConfiguration
    {
        public string Url { get; set; }

        public string DefaultDatatabaseName { get; set; }

        public bool Equals(IAbpRavenDbModuleConfiguration other)
        {
            if (other == null)
                return false;

            return string.Equals(Url, other.Url, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(DefaultDatatabaseName, other.DefaultDatatabaseName, StringComparison.OrdinalIgnoreCase);
        }
    }
}