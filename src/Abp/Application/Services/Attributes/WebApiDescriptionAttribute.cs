using System;

namespace Abp.Application.Services.Attributes
{
    /// <summary>
    /// Use it to description abp swagger
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface)]
    public class WebApiDescriptionAttribute : Attribute
    {
        /// <summary>
        /// Service Name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Brief Titile
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="description">Brief title</param>
        public WebApiDescriptionAttribute(string description)
        {
            Description = description;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Service Name</param>
        /// <param name="description">Brief title</param>
        public WebApiDescriptionAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
