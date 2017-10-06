namespace Abp.Zero.AspNetCore
{
    public enum SignInStatus
    {
        RequiresVerification,
        Success,
        Failure,
        LockedOut
    }
}