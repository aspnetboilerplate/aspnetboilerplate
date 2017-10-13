# Paths
$packFolder = (Get-Item -Path "./" -Verbose).FullName
$slnPath = Join-Path $packFolder "../"
$srcPath = Join-Path $slnPath "src"

# List of projects
$projects = (
    "Abp",
    "Abp.AspNetCore",
    "Abp.AutoMapper",
    "Abp.MailKit",
    "Abp.HangFire",
    "Abp.HangFire.AspNetCore",
    "Abp.Quartz",
    "Abp.EntityFramework.Common",
    "Abp.EntityFramework",
    "Abp.EntityFramework.GraphDiff",
    "Abp.EntityFrameworkCore",
    "Abp.Dapper",
    "Abp.FluentMigrator",
    "Abp.MemoryDb",
    "Abp.MongoDB",
    "Abp.NHibernate",
    "Abp.RedisCache",
    "Abp.RedisCache.ProtoBuf",
    "Abp.Owin",
    "Abp.Web.Common",
    "Abp.Web",
    "Abp.Web.Api",
    "Abp.Web.Mvc",
    "Abp.Web.SignalR",
    "Abp.Web.Api.OData",
    "Abp.Castle.Log4Net",
    "Abp.TestBase",
    "Abp.AspNetCore.TestBase",
    "Abp.Web.Resources",
    "Abp.Zero.Common",
    "Abp.Zero.Ldap",
    "Abp.Zero",
    "Abp.Zero.Owin",
    "Abp.Zero.AspNetCore",
    "Abp.Zero.EntityFramework",
    "Abp.Zero.EntityFrameworkCore",
    "Abp.Zero.NHibernate",
    "Abp.ZeroCore",
    "Abp.ZeroCore.EntityFrameworkCore",
    "Abp.ZeroCore.IdentityServer4",
    "Abp.ZeroCore.IdentityServer4.EntityFrameworkCore"    
)

# Rebuild solution
Set-Location $slnPath
& dotnet restore

# Copy all nuget packages to the pack folder
foreach($project in $projects) {
    
    $projectFolder = Join-Path $srcPath $project

    # Create nuget pack
    Set-Location $projectFolder
    Remove-Item -Recurse (Join-Path $projectFolder "bin/Release")
    & dotnet msbuild /t:pack /p:Configuration=Release /p:SourceLinkCreate=true

    # Copy nuget package
    $projectPackPath = Join-Path $projectFolder ("/bin/Release/" + $project + ".*.nupkg")
    Move-Item $projectPackPath $packFolder

}

# Go back to the pack folder
Set-Location $packFolder