using FluentMigrator.VersionTableInfo;

namespace ExamCenter.Data.Migrations
{
    [VersionTableMetaData]
    public class VersionTable : DefaultVersionTableMetaData
    {
        public override string TableName
        {
            get
            {
                return "ExamCenterVersionInfo";
            }
        }
    }
}
