namespace Abp.Web.Authorization
{
	/// <summary>
	/// This class is used to build and cache authrization script.
	/// </summary>
    public interface IAuthorizationScriptManager
    {
		/// <summary>
		/// Gets Javascript that contains all authorization information.
		/// </summary>
        string GetAuthorizationScript();
    }
}