using System.ComponentModel.DataAnnotations;

namespace Abp.WebApi.Runtime.Caching
{
    public class ClearCacheModel
    {
        [Required]
        public string Password { get; set; }

        public string[] Caches { get; set; }
    }
}