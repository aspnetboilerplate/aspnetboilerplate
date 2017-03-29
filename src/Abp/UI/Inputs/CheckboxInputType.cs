using System;
using Abp.Runtime.Validation;

namespace Abp.UI.Inputs
{
#if NET46
    [Serializable]
#endif
    [InputType("CHECKBOX")]
    public class CheckboxInputType : InputTypeBase
    {
        public CheckboxInputType()
            : this(new BooleanValueValidator())
        {

        }

        public CheckboxInputType(IValueValidator validator)
            : base(validator)
        {
            
        }
    }
}