using System;
using System.Collections.Generic;

namespace Abp.Auditing
{
    internal class AuditingConfiguration : IAuditingConfiguration
    {
        public bool IsEnabled { get; set; }

        public bool IsEnabledForAnonymousUsers { get; set; }

        public IMvcControllersAuditingConfiguration MvcControllers { get; } //TODO: Move to it's own package

        public IAuditingSelectorList Selectors { get; }

        public List<Type> IgnoredTypes { get; }

        public AuditingConfiguration()
        {
            IsEnabled = true;
            Selectors = new AuditingSelectorList();
            MvcControllers = new MvcControllersAuditingConfiguration();
            IgnoredTypes = new List<Type>();
        }
    }
}