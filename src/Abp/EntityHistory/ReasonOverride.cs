namespace Abp.EntityHistory
{
    public class ReasonOverride
    {
        public string Reason { get; }

        public ReasonOverride(string reason)
        {
            Reason = reason;
        }
    }
}
