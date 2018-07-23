using FluentMigrator.Runner.VersionTableInfo;

namespace Abp.Zero.FluentMigrator
{
    [VersionTableMetaData]
    public class VersionTable : DefaultVersionTableMetaData
    {
        public override string TableName
        {
            get
            {
                return "AbpVersionInfo";
            }
        }
    }
}
