namespace Abp.Dependency
{
	public interface IPostSharpOptions
	{
		/// <summary>
		/// Specifies if PostSharp aspects are enabled and ready to be used.
		/// </summary>
		bool EnablePostSharp { get; set; }
	}
}
