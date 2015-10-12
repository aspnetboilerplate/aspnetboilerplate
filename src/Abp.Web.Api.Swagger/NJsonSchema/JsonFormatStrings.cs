//-----------------------------------------------------------------------
// <copyright file="JsonFormatStrings.cs" company="NJsonSchema">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/rsuter/NJsonSchema/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NJsonSchema
{
    /// <summary>Class containing the constants available as format string. </summary>
    public static class JsonFormatStrings
    {
        /// <summary>Format for a <see cref="DateTime"/>. </summary>
        public const string DateTime = "date-time";

        /// <summary>Format for an email. </summary>
        public const string Email = "email";

        /// <summary>Format for an URI. </summary>
        public const string Uri = "uri";
    }
}