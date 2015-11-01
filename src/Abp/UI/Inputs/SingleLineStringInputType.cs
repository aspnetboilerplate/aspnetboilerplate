using System;
using Abp.Runtime.Validation;

namespace Abp.UI.Inputs
{
    [Serializable]
    [InputType("SINGLE_LINE_STRING")]
    public class SingleLineStringInputType : InputTypeBase
    {
        public SingleLineStringInputType()
        {

        }

        public SingleLineStringInputType(IValueValidator validator)
            : base(validator)
        {
        }
    }
}