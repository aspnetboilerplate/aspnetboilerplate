using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using Abp.Auditing;

namespace Abp.Web.Mvc.Auditing
{
    public class AbpAuditFilterData
    {
        private const string AbpAuditFilterDataHttpContextKey = "__AbpAuditFilterData";

        public Stopwatch Stopwatch { get; }

        public AuditInfo AuditInfo { get; }

        public AbpAuditFilterData(
            Stopwatch stopwatch,
            AuditInfo auditInfo)
        {
            Stopwatch = stopwatch;
            AuditInfo = auditInfo;
        }

        public static void Set(HttpContextBase httpContext, AbpAuditFilterData auditFilterData)
        {
            GetAuditDataStack(httpContext).Push(auditFilterData);
        }

        public static AbpAuditFilterData GetOrNull(HttpContextBase httpContext)
        {
            var stack = GetAuditDataStack(httpContext);
            return stack.Count <= 0
                ? null
                : stack.Pop();
        }

        private static Stack<AbpAuditFilterData> GetAuditDataStack(HttpContextBase httpContext)
        {
            var stack = httpContext.Items[AbpAuditFilterDataHttpContextKey] as Stack<AbpAuditFilterData>;

            if (stack == null)
            {
                stack = new Stack<AbpAuditFilterData>();
                httpContext.Items[AbpAuditFilterDataHttpContextKey] = stack;
            }

            return stack;
        }
    }
}