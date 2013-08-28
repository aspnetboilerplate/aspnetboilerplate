using FluentMigrator.VersionTableInfo;

namespace Abp.Data.Migrations.Core
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
