namespace Abp.Web.Models
{
    public class AbpErrorInfo
    {
        public string Title { get; set; }

        public string Message { get; set; }

        public AbpErrorInfo(string message)
            : this("Error!", message)
        {

        }
        
        public AbpErrorInfo(string title, string message)
        {
            Title = title;
            Message = message;
        }
    }
}