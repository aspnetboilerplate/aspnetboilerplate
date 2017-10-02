#tool "nuget:?package=xunit.runner.console&version=2.3.0-beta5-build3769"

#addin "nuget:?package=NuGet.Core"
#addin "nuget:?package=Cake.ExtendedNuGet"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var projectName = "Abp";
var solution = "./" + projectName + ".sln";

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var toolpath = Argument("toolpath", @"tools");
var branch = Argument("branch", EnvironmentVariable("APPVEYOR_REPO_BRANCH"));
var nugetApiKey = EnvironmentVariable("nugetApiKey");
var isRelease = EnvironmentVariable("APPVEYOR_REPO_TAG") == "true";

var testProjects = new List<Tuple<string, string[]>>
                {
                    new Tuple<string, string[]>("Abp.AspNetCore.Tests", new[] { "net461", "netcoreapp2.0" }),
                    new Tuple<string, string[]>("Abp.AutoMapper.Tests", new[] { "net461", "netcoreapp2.0" }),
                    new Tuple<string, string[]>("Abp.Castle.Log4Net.Tests", new[] { "net461", "netcoreapp2.0" }),
                    new Tuple<string, string[]>("Abp.Dapper.NHibernate.Tests", new[] { "net461"}),
                    new Tuple<string, string[]>("Abp.Dapper.Tests", new[] { "net461" }),
                    new Tuple<string, string[]>("Abp.EntityFramework.GraphDiff.Tests", new[] { "net461" }),
                    new Tuple<string, string[]>("Abp.EntityFramework.Tests", new[] { "net461" }),
                    new Tuple<string, string[]>("Abp.EntityFrameworkCore.Dapper.Tests", new[] { "netcoreapp2.0" }),
                    new Tuple<string, string[]>("Abp.EntityFrameworkCore.Tests", new[] { "net461", "netcoreapp2.0" }),
                    new Tuple<string, string[]>("Abp.MailKit.Tests", new[] { "net461", "netcoreapp2.0" }),
                    new Tuple<string, string[]>("Abp.MemoryDb.Tests", new[] { "net461", "netcoreapp2.0" }),
                    new Tuple<string, string[]>("Abp.NHibernate.Tests", new[] { "net461" }),
                    new Tuple<string, string[]>("Abp.Quartz.Tests", new[] { "net461" }),
                    new Tuple<string, string[]>("Abp.RedisCache.Tests", new[] { "net461", "netcoreapp2.0" }),
                    new Tuple<string, string[]>("Abp.TestBase.SampleApplication.Tests", new[] { "net461" }),
                    new Tuple<string, string[]>("Abp.Tests", new[] { "net461", "netcoreapp2.0" }),
                    new Tuple<string, string[]>("Abp.Web.Api.Tests", new[] { "net461" }),
                    new Tuple<string, string[]>("Abp.Web.Common.Tests", new[] { "net461", "netcoreapp2.0" }),
                    new Tuple<string, string[]>("Abp.Web.Mvc.Tests", new[] { "net461" }),
                    new Tuple<string, string[]>("Abp.Web.Tests", new[] { "net461" }),
                    new Tuple<string, string[]>("Abp.Zero.SampleApp.NHibernateTests", new[] { "net461" }),
                    new Tuple<string, string[]>("Abp.Zero.SampleApp.Tests", new[] { "net461" }),
                    new Tuple<string, string[]>("Abp.ZeroCore.Tests", new[] { "netcoreapp2.0" }),
                    new Tuple<string, string[]>("Abp.ZeroCore.IdentityServer4.Tests", new[] { "netcoreapp2.0" })
                };
                      

var nupkgPath = "nupkg";
var nupkgRegex = $"**/{projectName}*.nupkg";
var nugetPath = toolpath + "/nuget.exe";
var nugetQueryUrl = "https://www.nuget.org/api/v2/";
var nugetPushUrl = "https://www.nuget.org/api/v2/package";
var NUGET_PUSH_SETTINGS = new NuGetPushSettings
                          {
                              ToolPath = File(nugetPath),
                              Source = nugetPushUrl,
                              ApiKey = nugetApiKey
                          };

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
    {
        Information("Current Branch is:" + EnvironmentVariable("APPVEYOR_REPO_BRANCH"));
        Information($"IsRelease: {isRelease}");
        CleanDirectories("./src/**/bin");
        CleanDirectories("./src/**/obj");
        CleanDirectories("./test/**/bin");
        CleanDirectories("./test/**/obj");
        CleanDirectory(nupkgPath);
    });

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        DotNetCoreRestore(solution);
    });

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
    {
        MSBuild(solution, new MSBuildSettings(){Configuration = configuration}
                                               .WithProperty("SourceLinkCreate","true"));
    });

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
    {
        foreach (Tuple<string, string[]> testProject in testProjects)
        {
            foreach (string targetFramework in testProject.Item2)
            {
                Information($"Test execution started for target frameowork: {targetFramework}...");
                var testProj = GetFiles($"./test/**/*{testProject.Item1}.csproj").First();
                DotNetCoreTest(testProj.FullPath, new DotNetCoreTestSettings { Configuration = "Release", Framework = targetFramework });                              
            }
        }
    });
    
Task("Pack")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
    {
        var nupkgFiles = GetFiles(nupkgRegex);
        MoveFiles(nupkgFiles, nupkgPath);
    });

Task("NugetPublish")
    .IsDependentOn("Pack")
    .WithCriteria(() => branch == "master" && isRelease)
    .Does(()=>
    {
        foreach(var nupkgFile in GetFiles(nupkgRegex))
        {
          if(!IsNuGetPublished(nupkgFile, nugetQueryUrl))
          {
             Information("Publishing... " + nupkgFile);
             NuGetPush(nupkgFile, NUGET_PUSH_SETTINGS);
          }
          else
          {
             Information("Already published, skipping... " + nupkgFile);
          }
        }
    });

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build")
    .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Pack")
    .IsDependentOn("NugetPublish");
    
//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
