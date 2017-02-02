param (
    $versionSuffix,
    $slnPath,
    $targetPath
)

if($slnPath -eq $null) { $slnPath = "..\" }
if($targetPath -eq $null) { $targetPath = "." }

$srcPath = $slnPath + "src\"

$projects = (
    "Abp",
    "Abp.AspNetCore",
    "Abp.AutoMapper",
    "Abp.HangFire",
    "Abp.Quartz",
    "Abp.EntityFramework.Common",
    "Abp.EntityFramework",
    "Abp.EntityFramework.GraphDiff",
    "Abp.EntityFrameworkCore",
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
    "Abp.Web.Resources"
)

foreach($project in $projects) {
    $prjPath = $srcPath + $project
    if($versionSuffix -eq $null) {
        & dotnet "pack" "$prjPath" -c "Release" -o "$targetPath"
    } else {
        & dotnet "pack" "$prjPath" -c "Release" -o "$targetPath" --version-suffix "$versionSuffix"
    }
}