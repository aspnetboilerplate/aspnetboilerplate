# Paths
$packFolder = (Get-Item -Path "./" -Verbose).FullName
$slnPath = Join-Path $packFolder "../"
$srcPath = Join-Path $slnPath "src"

# List of projects
$projects = (
    "Abp",
    "Abp.AspNetCore",
    "Abp.AspNetCore.OData",
    "Abp.AspNetCore.SignalR",
    "Abp.AspNetCore.TestBase",
    "Abp.AspNetCore.PerRequestRedisCache",
    "Abp.AutoMapper",
    "Abp.BlobStoring",
    "Abp.BlobStoring.Azure",
    "Abp.BlobStoring.FileSystem",
    "Abp.Castle.Log4Net",
    "Abp.Dapper",
    "Abp.EntityFramework",
    "Abp.EntityFramework.Common",
    "Abp.EntityFrameworkCore",
    "Abp.FluentMigrator",
    "Abp.FluentValidation",
    "Abp.HangFire",
    "Abp.HangFire.AspNetCore",
    "Abp.HtmlSanitizer",
    "Abp.MailKit",
    "Abp.MemoryDb",
    "Abp.MongoDB",
    "Abp.NHibernate",
    "Abp.RedisCache",
    "Abp.RedisCache.ProtoBuf",
    "Abp.Quartz",
    "Abp.TestBase",
    "Abp.Web.Common",
    "Abp.Web.Resources",
    "Abp.Zero.Common",
    "Abp.Zero.Ldap",
    "Abp.ZeroCore",
    "Abp.ZeroCore.EntityFramework",
    "Abp.ZeroCore.EntityFrameworkCore",
    "Abp.ZeroCore.IdentityServer4.vNext",
    "Abp.ZeroCore.IdentityServer4.vNext.EntityFrameworkCore",
    "Abp.ZeroCore.NHibernate",
	"Abp.ZeroCore.OpenIddict",
	"Abp.ZeroCore.OpenIddict.EntityFrameworkCore",
	"Abp.AspNetCore.OpenIddict"
)

# Rebuild solution
Set-Location $slnPath
& dotnet restore

# Copy all nuget packages to the pack folder
foreach($project in $projects) {
    
    $projectFolder = Join-Path $srcPath $project

    # Create nuget pack
    Set-Location $projectFolder
    Get-ChildItem (Join-Path $projectFolder "bin/Release") -ErrorAction SilentlyContinue | Remove-Item -Recurse
    & dotnet msbuild /p:Configuration=Release
    & dotnet msbuild /p:Configuration=Release /t:pack /p:IncludeSymbols=true /p:SymbolPackageFormat=snupkg

    # Copy nuget package
    $projectPackPath = Join-Path $projectFolder ("/bin/Release/" + $project + ".*.nupkg")
    Move-Item $projectPackPath $packFolder

	# Copy symbol package
    $projectPackPath = Join-Path $projectFolder ("/bin/Release/" + $project + ".*.snupkg")
    Move-Item $projectPackPath $packFolder
}

# Go back to the pack folder
Set-Location $packFolder