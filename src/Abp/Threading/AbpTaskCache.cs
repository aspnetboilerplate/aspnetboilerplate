using System.Threading.Tasks;

namespace Abp.Threading
{
    public static class AbpTaskCache
    {
        public static Task CompletedTask { get; } = Task.FromResult(0);
    }
}
