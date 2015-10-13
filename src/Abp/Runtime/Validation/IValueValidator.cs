namespace Abp.Runtime.Validation
{
    public interface IValueValidator
    {
        bool IsValid(object value);
    }
}