namespace Abp.Zero.SampleApp.Users.Dto
{
    public class ResetPasswordInput
    {
        public int? TenantId { get; set; }

        public long UserId { get; set; }

        public string ResetCode { get; set; }

        public string Password { get; set; }
    }
}
