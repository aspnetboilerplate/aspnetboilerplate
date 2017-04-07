namespace Abp.Tests.Events.Bus
{
    public class MyDerivedEventData : MySimpleEventData
    {
        public MyDerivedEventData(int value)
            : base(value)
        {
        }
    }
}