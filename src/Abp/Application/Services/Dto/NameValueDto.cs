namespace Abp.Application.Services.Dto
{
    /// <summary>
    /// Can be used to send/receive Name/Value (or Key/Value) pairs.
    /// </summary>
    public class NameValueDto : IDto
    {
        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Creates a new <see cref="NameValueDto"/>.
        /// </summary>
        public NameValueDto()
        {

        }

        /// <summary>
        /// Creates a new <see cref="NameValueDto"/>.
        /// </summary>
        public NameValueDto(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}