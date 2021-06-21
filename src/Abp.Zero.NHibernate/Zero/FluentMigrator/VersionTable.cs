using FluentMigrator.Runner.VersionTableInfo;
using System;

namespace Abp.Zero.FluentMigrator
{
    [VersionTableMetaData]
    public class VersionTable : DefaultVersionTableMetaData
    {
        [Obsolete("Use dependency injection")]
        public VersionTable() : base()
        {
        }

        [Obsolete("Use dependency injection")]
        public VersionTable(string schemaName) : base(schemaName)
        {
        }

        public override string TableName
        {
            get
            {
                return "AbpVersionInfo";
            }
        }
    }
}
