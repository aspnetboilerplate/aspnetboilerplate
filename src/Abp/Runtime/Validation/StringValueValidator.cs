using System;
using System.Text.RegularExpressions;
using Castle.Core.Internal;

namespace Abp.Runtime.Validation
{
    [Serializable]
    [Validator("STRING")]
    public class StringValueValidator : IValueValidator
    {
        public bool AllowNull { get; set; }

        public int MinLength { get; set; }

        public int MaxLength { get; set; }

        public string RegularExpression { get; set; }

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

        public virtual bool IsValid(object value)
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
            
            if (MinLength > 0 && strValue.Length <= 0)
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