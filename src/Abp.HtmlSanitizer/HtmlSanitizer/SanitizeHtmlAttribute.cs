namespace Abp.HtmlSanitizer.HtmlSanitizer
{
    /// <summary>
    /// Can be added to a method to enable auto validation if validation is disabled for it's class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
    public class SanitizeHtmlAttribute : Attribute
    {
        public bool IsDisabled { get; set; }

        public bool KeepChildNodes
        {
            get => Ganss.Xss.HtmlSanitizer.DefaultKeepChildNodes;
            set => Ganss.Xss.HtmlSanitizer.DefaultKeepChildNodes = value;
        }
    }
}

