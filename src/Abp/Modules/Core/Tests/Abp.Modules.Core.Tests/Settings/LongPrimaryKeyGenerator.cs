using System.Threading;

namespace Abp.Modules.Core.Tests.Settings
{
    public class LongPrimaryKeyGenerator : IPrimaryKeyGenerator<long>
    {
        private long _lastId;

        public long Generate()
        {
            return Interlocked.Increment(ref _lastId);
        }
    }
}