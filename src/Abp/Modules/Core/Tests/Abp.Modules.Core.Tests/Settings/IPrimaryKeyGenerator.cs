namespace Abp.Modules.Core.Tests.Settings
{
    public interface IPrimaryKeyGenerator<TPrimaryKey>
    {
        TPrimaryKey Generate();
    }
}