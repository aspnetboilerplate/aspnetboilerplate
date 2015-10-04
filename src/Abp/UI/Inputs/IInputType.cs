using Abp.Runtime.Validation;

namespace Abp.UI.Inputs
{
    public interface IInputType
    {
        string Name { get; }

        object this[string key] { get; set; }

        IValueValidator Validator { get; set; }
    }
}