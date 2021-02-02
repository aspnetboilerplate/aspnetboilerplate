using System;
using Abp.Dependency;

namespace Abp.MimeTypes
{
    public interface IMimeTypeMap
    {
        /// <summary>
        /// Tries to get the type of the MIME from the provided string.
        /// </summary>
        /// <param name="str">The filename or extension.</param>
        /// <param name="mimeType">The variable to store the MIME type.</param>
        /// <returns>Whether the transaction was completed successfully.</returns>
        /// <exception cref="ArgumentNullException" />
        bool TryGetMimeType(string str, out string mimeType);
        
        /// <summary>
        /// Gets the type of the MIME from the provided string.
        /// </summary>
        /// <param name="str">The filename or extension.</param>
        /// <param name="throwErrorIfNotFound">if set to <c>true</c>, throws error if extension's not found.</param>
        /// <returns>The MIME type.</returns>
        /// <exception cref="ArgumentNullException" />
        string GetMimeType(string str, bool throwErrorIfNotFound = true);
        
        /// <summary>
        /// Gets the extension from the provided MIME type.
        /// </summary>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <param name="extension">The variable to store the extension.</param>
        /// <returns>Whether the transaction was completed successfully.</returns>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentException" />
        bool TryGetExtension(string mimeType, out string extension);
        
        /// <summary>
        /// Gets the extension from the provided MIME type.
        /// </summary>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <param name="throwErrorIfNotFound">if set to <c>true</c>, throws error if extension's not found.</param>
        /// <returns>The extension.</returns>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentException" />
        string GetExtension(string mimeType, bool throwErrorIfNotFound = true);
        
        /// <summary>
        /// Adds MIME type to map
        /// </summary>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <param name="extension">Type of the extension</param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentException" />
        void AddMimeType(string mimeType, string extension);
        
        /// <summary>
        /// Removes MIME type from map
        /// </summary>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentException" />
        void RemoveMimeType(string mimeType);
        
        /// <summary>
        /// Adds extension to map
        /// </summary>
        /// <param name="extension">Type of the extension</param>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentException" />
        void AddExtension(string extension, string mimeType);
        
        /// <summary>
        /// Removes extension from map
        /// </summary>
        /// <param name="extension">Type of the extension</param>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentException" />
        void RemoveExtension(string extension);
    }
}