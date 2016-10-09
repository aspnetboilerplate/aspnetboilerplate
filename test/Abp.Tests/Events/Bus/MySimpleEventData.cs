using Abp.Events.Bus;

namespace Abp.Tests.Events.Bus
{
    public class MySimpleEventData : EventData
    {
        public int Value { get; set; }

        public MySimpleEventData(int value)
        {
            Value = value;
        }
    }
}