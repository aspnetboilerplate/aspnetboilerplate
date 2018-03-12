namespace Abp.Events.Bus.Entities
{
    public enum EntityChangeType : byte
    {
        Created = 0,
        Updated = 1,
        Deleted = 2
    }
}
