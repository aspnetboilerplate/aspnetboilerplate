using JetBrains.Annotations;

namespace Abp.Domain.Entities
{
    /// <summary>
    /// Defines a JSON formatted string property to extend an object/entity.
    /// </summary>
    public interface IExtendableObject
    {
        /// <summary>
        /// A JSON formatted string to extend the containing object.
        /// JSON data can contain properties with arbitrary values (like primitives or complex objects).
        /// Extension methods are available (<see cref="ExtendableObjectExtensions"/>) to manipulate this data.
        /// General format:
        /// <code>
        /// {
        ///   "Property1" : ...
        ///   "Property2" : ...
        /// }
        /// </code>
        /// </summary>
        [CanBeNull]
        string ExtensionData { get; set; }
    }
}
