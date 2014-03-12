using FluentMigrator.VersionTableInfo;

namespace MySpaProject.DbMigrations
{
    [VersionTableMetaData]
    public class VersionTable : DefaultVersionTableMetaData
    {
        public override string TableName
        {
            get
            {
                return "TsVersionInfo";
            }
        }
    }
}
