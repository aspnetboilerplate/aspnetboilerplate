using FluentMigrator.VersionTableInfo;

namespace Taskever.Data.Migrations
{
    [VersionTableMetaData]
    public class VersionTable : DefaultVersionTableMetaData
    {
        public override string TableName
        {
            get
            {
                return "TeVersionInfo";
            }
        }
    }
}
