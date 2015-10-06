using System;
using Abp.Extensions;

namespace Abp.Runtime.Validation
{
    [Serializable]
    [Validator("NUMERIC")]
    public class NumericValueValidator : IValueValidator
    {
        public int MinValue { get; set; }

        public int MaxValue { get; set; }

        public NumericValueValidator()
        {

        }

        public NumericValueValidator(int minValue = int.MinValue, int maxValue = int.MaxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (value is int)
            {
                return IsValidInternal((int)value);
            }

            if (value is string)
            {
                int intValue;
                if (int.TryParse(value as string, out intValue))
                {
                    return IsValidInternal(intValue);
                }
            }

            return false;
        }

        protected virtual bool IsValidInternal(int value)
        {
            return value.IsBetween(MinValue, MaxValue);
        }
    }
}