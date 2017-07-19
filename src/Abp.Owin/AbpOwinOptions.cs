namespace Abp.Owin
{
    public class AbpOwinOptions
    {
        /// <summary>
        /// Default: true.
        /// </summary>
        public bool UseEmbeddedFiles { get; set; }

        public AbpOwinOptions()
        {
            UseEmbeddedFiles = true;
        }
    }
}