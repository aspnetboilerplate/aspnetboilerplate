namespace Abp.Auditing
{
    public class NullClientInfoProvider : IClientInfoProvider
    {
        public static NullClientInfoProvider Instance { get; } = new NullClientInfoProvider();

        public string BrowserInfo => null;
        public string ClientIpAddress => null;
        public string ComputerName => null;
    }
}