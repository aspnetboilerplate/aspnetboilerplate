namespace Taskever.Domain.Entities.EventHistories
{
    public abstract class EventHistoryFormatter
    {
        public abstract EventHistoryType Type { get; }
        public abstract short LatestVersion { get; }
        public abstract EventHistory CreateEventHistory();
        public abstract void WriteTo(EventHistory history);
        public abstract void ReadFrom(EventHistory history);
    }
}