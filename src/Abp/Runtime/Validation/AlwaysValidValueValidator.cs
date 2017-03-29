using System;

namespace Abp.Runtime.Validation
{
    [Validator("NULL")]
#if NET46
    [Serializable]
#endif
    public class AlwaysValidValueValidator : ValueValidatorBase
    {
        public override bool IsValid(object value)
        {
            return true;
        }
    }
}