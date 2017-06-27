using System;

namespace Sample.MqMessages
{
    /// <summary>
    /// Custom MqMessage Definition. No depends on any framework,this class library can be shared as nuget pkg.
    /// </summary>
    public class TestMqMessage
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public DateTime Time { get; set; }
    }
}
