namespace Abp.Web.Api.Tests.AppServices
{
    public class MyFirstAppService : IMyFirstAppService
    {
        public string GetStr(int i)
        {
            return i.ToString();
        }

        public string GetStr2(int i)
        {
            return (i + 1).ToString();
        }
    }
}