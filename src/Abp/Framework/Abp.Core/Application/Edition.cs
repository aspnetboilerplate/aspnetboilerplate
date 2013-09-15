using System;
using System.Collections.Generic;

namespace Abp.Application
{
    /// <summary>
    /// Represents an edition of the application.
    /// </summary>
    public class Edition
    {
        /// <summary>
        /// Unique name of the edition.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Display name of this edition.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// All features of this edition.
        /// </summary>
        public IDictionary<string, Feature> Features { get; private set; }
        
        [ThreadStatic]
        private static Edition _current;
        public static Edition Current
        {
            get { return _current ?? new Edition("Standard"); } //TODO: Remove dummy entity
            set { _current = value; }
        }
        
        /// <summary>
        /// Create a new Edition.
        /// </summary>
        /// <param name="name">Unique name of the edition.</param>
        public Edition(string name)
        {
            Name = name;
            Features = new Dictionary<string, Feature>();
        }
    }
}
