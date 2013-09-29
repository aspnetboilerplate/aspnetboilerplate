namespace Abp.Web.Mvc.Models
{
    public class ErrorModel
    {
        public string Title { get; set; }

        public string Message { get; set; }

        public ErrorModel(string message)
            : this("Error!", message)
        {

        }

        public ErrorModel(string title, string message)
        {
            Title = title;
            Message = message;
        }
    }
}