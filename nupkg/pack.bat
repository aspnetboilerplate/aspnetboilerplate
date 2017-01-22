REM "..\tools\gitlink\GitLink.exe" ..\ -u https://github.com/aspnetboilerplate/aspnetboilerplate -c release

@ECHO OFF
SET /P VERSION_SUFFIX=Please enter version-suffix (can be left empty): 

dotnet "pack" "..\src\Abp" -c "Release" -o "." --version-suffix "%VERSION_SUFFIX%"
dotnet "pack" "..\src\Abp.AspNetCore" -c "Release" -o "." --version-suffix "%VERSION_SUFFIX%"
dotnet "pack" "..\src\Abp.AutoMapper" -c "Release" -o "." --version-suffix "%VERSION_SUFFIX%"
dotnet "pack" "..\src\Abp.HangFire" -c "Release" -o "." --version-suffix "%VERSION_SUFFIX%"
dotnet "pack" "..\src\Abp.Quartz" -c "Release" -o "." --version-suffix "%VERSION_SUFFIX%"
dotnet "pack" "..\src\Abp.EntityFramework.Common" -c "Release" -o "." --version-suffix "%VERSION_SUFFIX%"
dotnet "pack" "..\src\Abp.EntityFramework" -c "Release" -o "." --version-suffix "%VERSION_SUFFIX%"
dotnet "pack" "..\src\Abp.EntityFramework.GraphDiff" -c "Release" -o "." --version-suffix "%VERSION_SUFFIX%"
dotnet "pack" "..\src\Abp.EntityFrameworkCore" -c "Release" -o "." --version-suffix "%VERSION_SUFFIX%"
dotnet "pack" "..\src\Abp.FluentMigrator" -c "Release" -o "." --version-suffix "%VERSION_SUFFIX%"
dotnet "pack" "..\src\Abp.MemoryDb" -c "Release" -o "." --version-suffix "%VERSION_SUFFIX%"
dotnet "pack" "..\src\Abp.MongoDB" -c "Release" -o "." --version-suffix "%VERSION_SUFFIX%"
dotnet "pack" "..\src\Abp.NHibernate" -c "Release" -o "." --version-suffix "%VERSION_SUFFIX%"
dotnet "pack" "..\src\Abp.RedisCache" -c "Release" -o "." --version-suffix "%VERSION_SUFFIX%"
dotnet "pack" "..\src\Abp.RedisCache.ProtoBuf" -c "Release" -o "." --version-suffix "%VERSION_SUFFIX%"
dotnet "pack" "..\src\Abp.Owin" -c "Release" -o "." --version-suffix "%VERSION_SUFFIX%"
dotnet "pack" "..\src\Abp.Web.Common" -c "Release" -o "." --version-suffix "%VERSION_SUFFIX%"
dotnet "pack" "..\src\Abp.Web" -c "Release" -o "." --version-suffix "%VERSION_SUFFIX%"
dotnet "pack" "..\src\Abp.Web.Api" -c "Release" -o "." --version-suffix "%VERSION_SUFFIX%"
dotnet "pack" "..\src\Abp.Web.Mvc" -c "Release" -o "." --version-suffix "%VERSION_SUFFIX%"
dotnet "pack" "..\src\Abp.Web.SignalR" -c "Release" -o "." --version-suffix "%VERSION_SUFFIX%"
dotnet "pack" "..\src\Abp.Web.Api.OData" -c "Release" -o "." --version-suffix "%VERSION_SUFFIX%"
dotnet "pack" "..\src\Abp.Castle.Log4Net" -c "Release" -o "." --version-suffix "%VERSION_SUFFIX%"
dotnet "pack" "..\src\Abp.TestBase" -c "Release" -o "." --version-suffix "%VERSION_SUFFIX%"
dotnet "pack" "..\src\Abp.AspNetCore.TestBase" -c "Release" -o "." --version-suffix "%VERSION_SUFFIX%"
dotnet "pack" "..\src\Abp.Web.Resources" -c "Release" -o "." --version-suffix "%VERSION_SUFFIX%"

pause
