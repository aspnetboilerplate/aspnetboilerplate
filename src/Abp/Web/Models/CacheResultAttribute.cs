using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Web.Models
{
    /// <summary>
    /// Used to determine how response should be cached
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method)]
    public class CacheResultAttribute : Attribute
    {
        public CacheResultAttribute()
        {
        }
        CacheResultAttribute(int maxAge, bool privateOnly = false, bool mustRevalidate = true)
            : this(maxAge)
        {
            PrivateOnly = privateOnly;
            MustRevalidate = mustRevalidate;
        }
        public CacheResultAttribute(int maxAge)
        {
            NoCache = maxAge > 0;
            MaxAge = maxAge;
        }

        public bool NoCache { get; set; } = true;
        public int MaxAge { get; set; } = 0;
        public bool MustRevalidate { get; set; } = true;
        public bool PrivateOnly { get; set; } = true;
    }
}
