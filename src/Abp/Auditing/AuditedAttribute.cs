using System;

namespace Abp.Auditing
{
    /* NOTE: THIS CODES ARE UNDER WORK AND NOT ACTIVE YET!
     */

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    internal class AuditedAttribute : Attribute
    {
        /// <summary>
        /// Default value: <see cref="AuditUsage.Default"/>
        /// </summary>
        public AuditUsage Usage { get; set; }

        public AuditedAttribute()
        {
            Usage = AuditUsage.Default;
        }

        public AuditedAttribute(AuditUsage usage)
        {
            Usage = usage;
        }
    }

    internal enum AuditUsage
    {
        Default,
        Enabled,
        Disabled
    }

    internal class AuditInfo
    {
        public string ServiceName { get; set; }
        public string MethodName { get; set; }
        public int Duration { get; set; }
        public DateTime ExecutionTime { get; set; }
        public string Parameters { get; set; }
        public string CustomData { get; set; } //TODO: ...?
        public string BrowserInfo { get; set; }
        public string ClientName { get; set; } //Client computer's name
        public string ClientIpAddress { get; set; }
        public string ExceptionInfo { get; set; }
        public long? UserId { get; set; }
        public int? TenantId { get; set; }
    }
}
