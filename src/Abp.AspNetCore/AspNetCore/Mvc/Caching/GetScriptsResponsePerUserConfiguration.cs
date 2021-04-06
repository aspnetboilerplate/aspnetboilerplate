using System;

namespace Abp.AspNetCore.Mvc.Caching
{
    internal class GetScriptsResponsePerUserConfiguration : IGetScriptsResponsePerUserConfiguration
    {
        public bool IsEnabled { get; set; }
        public TimeSpan MaxAge { get; set; } = TimeSpan.FromMinutes(30);
    }
}