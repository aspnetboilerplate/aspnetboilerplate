using Abp.Runtime.Validation;
using System;

namespace Abp.UI.Inputs
{
    [Serializable]
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