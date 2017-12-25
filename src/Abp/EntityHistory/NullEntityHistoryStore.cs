using System.Threading.Tasks;

namespace Abp.EntityHistory
{
    public class NullEntityHistoryStore : IEntityHistoryStore
    {
        public static NullEntityHistoryStore Instance { get; } = new NullEntityHistoryStore();

        public Task SaveAsync(EntityChangeInfo entityChangeInfo)
        {
            return Task.CompletedTask;
        }

        public Task SaveAsync(EntityChangeSet entityChangeSet)
        {
            return Task.CompletedTask;
        }
    }
}
