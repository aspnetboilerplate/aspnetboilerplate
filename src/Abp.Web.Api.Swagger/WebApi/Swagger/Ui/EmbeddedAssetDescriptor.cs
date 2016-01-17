using System.Reflection;

namespace Abp.WebApi.Swagger.Ui
{
    /// <summary>
    /// Embedded Resource Descriptor
    /// </summary>
    public class EmbeddedAssetDescriptor
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="containingAssembly">assembly of your application</param>
        /// <param name="name">your resource name</param>
        /// <param name="isTemplate">Indicating if it is a template parameter</param>
        public EmbeddedAssetDescriptor(Assembly containingAssembly, string name, bool isTemplate = false)
        {
            Assembly = containingAssembly;
            Name = name;
            IsTemplate = isTemplate;
        }

        /// <summary>
        /// assembly of your application
        /// </summary>
        public Assembly Assembly { get; private set; }
        /// <summary>
        /// resource name
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Indicating if it is a template parameter
        /// </summary>
        public bool IsTemplate { get; private set; }
    }
}