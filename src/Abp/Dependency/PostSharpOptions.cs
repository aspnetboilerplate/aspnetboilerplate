namespace Abp.Dependency
{
	internal class PostSharpOptions : IPostSharpOptions
	{
		/// <summary>
		/// Specifies if PostSharp aspects are enabled and ready to be used.
		/// Only <see cref="AbpBootstrapper"/> should change this value.
		/// </summary>
		public bool EnablePostSharp { get; set; } = false;
	}
}
