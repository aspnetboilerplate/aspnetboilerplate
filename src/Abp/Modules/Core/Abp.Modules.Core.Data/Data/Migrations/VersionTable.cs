using FluentMigrator.VersionTableInfo;

namespace Abp.Modules.Core.Data.Migrations
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
