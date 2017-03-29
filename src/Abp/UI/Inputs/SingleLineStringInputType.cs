using System;
using Abp.Runtime.Validation;

namespace Abp.UI.Inputs
{
#if NET46
    [Serializable]
#endif
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