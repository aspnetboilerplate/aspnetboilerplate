using FluentMigrator.Runner.Conventions;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.Options;
using System;

namespace Abp.Zero.FluentMigrator;

[VersionTableMetaData]
public class VersionTable : DefaultVersionTableMetaData
{
    public VersionTable(IConventionSet conventionSet, IOptions<RunnerOptions> options)
        : base(conventionSet, options)
    {
    }

    [Obsolete("Use dependency injection")]
    public VersionTable(IConventionSet conventionSet, IOptions<RunnerOptions> options, string schemaName)
        : base(conventionSet, options)
    {
        SchemaName = schemaName;
    }

    public override string TableName
    {
        get
        {
            return "AbpVersionInfo";
        }
    }
}