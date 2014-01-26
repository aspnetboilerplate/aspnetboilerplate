namespace Taskever.Web.Mvc.Models.Account
{
    public class ResetPasswordViewModel
    {
        public int UserId { get; set; }

        public string ResetCode { get; set; }
    }
}