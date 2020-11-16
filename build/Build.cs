using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.AppVeyor;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
[MSBuildVerbosityMapping]
[AzurePipelines(
    AzurePipelinesImage.WindowsLatest,
    InvokedTargets = new[] {nameof(Test)},
    ExcludedTargets = new[] {nameof(Clean)},
    NonEntryTargets = new[] {nameof(Restore), nameof(Compile)})]
[AppVeyor(
    AppVeyorImage.VisualStudioLatest,
    InvokedTargets = new[] {nameof(Test)},
    AutoGenerate = false)]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [CI] readonly AzurePipelines AzurePipelines;
    [Solution] readonly Solution Solution;

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            RootDirectory
                .GlobDirectories(
                    "*/src/*/obj",
                    "*/src/*/bin",
                    "*/test/*/obj",
                    "*/test/*/bin")
                .ForEach(DeleteDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(_ => _
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(_ => _
                .SetProjectFile(Solution)
                .SetNoRestore(InvokedTargets.Contains(Restore))
                .SetConfiguration(Configuration)
                .SetProperty("SourceLinkCreate", true));
        });

    static AbsolutePath PackagesDirectory => RootDirectory / "output" / "packages";

    Target Pack => _ => _
        .DependsOn(Compile)
        .Produces(PackagesDirectory / "*.nupkg")
        .Executes(() =>
        {
            DotNetPack(_ => _
                .SetConfiguration(Configuration)
                .SetOutputDirectory(PackagesDirectory)
                .SetNoBuild(InvokedTargets.Contains(Compile))
                .SetProperty("SourceLinkCreate", true)
                .CombineWith(
                    Solution.AllProjects.Where(x => x.SolutionFolder?.Name == "src"), (_, v) => _
                        .SetProject(v)));
        });

    [Partition(4)] readonly Partition TestPartition;

    AbsolutePath TestResultDirectory => RootDirectory / "output" / "test-results";

    Target Test => _ => _
        .DependsOn(Compile)
        .Partition(() => TestPartition)
        .Executes(() =>
        {
            var allTestConfigurations =
                from project in Solution.GetProjects("*Tests")
                from targetFramework in project.GetTargetFrameworks()
                select (project, targetFramework);
            var relevantTestConfigurations = TestPartition.GetCurrent(allTestConfigurations);

            try
            {
                DotNetTest(_ => _
                        .SetConfiguration(Configuration.Release)
                        .SetNoBuild(InvokedTargets.Contains(Compile))
                        .SetResultsDirectory(TestResultDirectory)
                        .CombineWith(relevantTestConfigurations, (_, v) => _
                            .SetProjectFile(v.project)
                            .SetFramework(v.targetFramework)
                            .SetLogger($"trx;LogFileName={v.project.Name}.trx")),
                    completeOnFailure: true);
            }
            finally
            {
                TestResultDirectory.GlobFiles("*.trx").ForEach(x =>
                    AzurePipelines?.PublishTestResults(
                        type: AzurePipelinesTestResultsType.VSTest,
                        title: $"{Path.GetFileNameWithoutExtension(x)} ({AzurePipelines.StageDisplayName})",
                        files: new string[] { x }));
            }
        });
}
