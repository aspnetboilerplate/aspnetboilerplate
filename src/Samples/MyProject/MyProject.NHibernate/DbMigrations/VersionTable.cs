using FluentMigrator.VersionTableInfo;

namespace MyProject.DbMigrations
{
    [VersionTableMetaData]
    public class VersionTable : DefaultVersionTableMetaData
    {
        public override string TableName
        {
            get
            {
                return "MyAppVersionInfo";
            }
        }
    }
}
