using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Abp.Extensions;

namespace Abp.Runtime.Validation
{
    [Serializable]
    [Validator("STRING")]
    public class StringValueValidator : ValueValidatorBase
    {
        public bool AllowNull
        {
            get { return (this["AllowNull"] ?? "false").To<bool>(); }
            set { this["AllowNull"] = value.ToString().ToLowerInvariant(); }
        }

        public int MinLength
        {
            get { return (this["MinLength"] ?? "0").To<int>(); }
            set { this["MinLength"] = value; }
        }

        public int MaxLength
        {
            get { return (this["MaxLength"] ?? "0").To<int>(); }
            set { this["MaxLength"] = value; }
        }

        public string RegularExpression
        {
            get { return this["RegularExpression"] as string; }
            set { this["RegularExpression"] = value; }
        }

        public StringValueValidator()
        {
            
        }

        public StringValueValidator(int minLength = 0, int maxLength = 0, string regularExpression = null, bool allowNull = false)
        {
            MinLength = minLength;
            MaxLength = maxLength;
            RegularExpression = regularExpression;
            AllowNull = allowNull;
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return AllowNull;
            }

            if (!(value is string))
            {
                return false;
            }

            var strValue = value as string;
            
            if (MinLength > 0 && strValue.Length < MinLength)
            {
                return false;
            }

            if (MaxLength > 0 && strValue.Length > MaxLength)
            {
                return false;
            }

            if (!RegularExpression.IsNullOrEmpty())
            {
                return Regex.IsMatch(strValue, RegularExpression);
            }

            return true;
        }
    }
}