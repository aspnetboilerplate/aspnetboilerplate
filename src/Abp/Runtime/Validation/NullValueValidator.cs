namespace Abp.Runtime.Validation
{
    [Validator("NULL")]
    public class NullValueValidator : IValueValidator
    {
        public bool IsValid(object value)
        {
            return true;
        }
    }
}