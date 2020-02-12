namespace Abp.Web.Configuration
{
    /// <summary>
    /// Used to create client scripts for custom config.
    /// </summary>
    public interface ICustomConfigScriptManager
    {
        string GetScript();
    }
}
