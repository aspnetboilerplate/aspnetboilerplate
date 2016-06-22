namespace Abp.AspNetCore.Tests.App.Models
{
    public class SimpleViewModel
    {
        public string StrValue { get; set; }

        public int IntValue { get; set; }

        public SimpleViewModel()
        {
            
        }

        public SimpleViewModel(string strValue, int intValue)
        {
            StrValue = strValue;
            IntValue = intValue;
        }
    }
}
