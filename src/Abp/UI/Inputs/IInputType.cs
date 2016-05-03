using Abp.Runtime.Validation;
using System.Collections.Generic;

namespace Abp.UI.Inputs
{
    public interface IInputType
    {
        string Name { get; }

        object this[string key] { get; set; }

        IDictionary<string, object> Attributes { get; }

        IValueValidator Validator { get; set; }
    }
}