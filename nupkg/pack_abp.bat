"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp\Abp.csproj" -Properties Configuration=Release -IncludeReferencedProjects -Symbols
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp.AutoMapper\Abp.AutoMapper.csproj" -Properties Configuration=Release -IncludeReferencedProjects -Symbols
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp.HangFire\Abp.HangFire.csproj" -Properties Configuration=Release -IncludeReferencedProjects -Symbols
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp.EntityFramework\Abp.EntityFramework.csproj" -Properties Configuration=Release -IncludeReferencedProjects -Symbols
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp.FluentMigrator\Abp.FluentMigrator.csproj" -Properties Configuration=Release -IncludeReferencedProjects -Symbols
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp.MemoryDb\Abp.MemoryDb.csproj" -Properties Configuration=Release -IncludeReferencedProjects -Symbols
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp.MongoDB\Abp.MongoDB.csproj" -Properties Configuration=Release -IncludeReferencedProjects -Symbols
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp.NHibernate\Abp.NHibernate.csproj" -Properties Configuration=Release -IncludeReferencedProjects -Symbols
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp.RedisCache\Abp.RedisCache.csproj" -Properties Configuration=Release -IncludeReferencedProjects -Symbols
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp.Owin\Abp.Owin.csproj" -Properties Configuration=Release -IncludeReferencedProjects -Symbols
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp.Web\Abp.Web.csproj" -Properties Configuration=Release -IncludeReferencedProjects -Symbols
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp.Web.Api\Abp.Web.Api.csproj" -Properties Configuration=Release -IncludeReferencedProjects -Symbols
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp.Web.Mvc\Abp.Web.Mvc.csproj" -Properties Configuration=Release -IncludeReferencedProjects -Symbols
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp.Web.SignalR\Abp.Web.SignalR.csproj" -Properties Configuration=Release -IncludeReferencedProjects -Symbols
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp.Web.Api.OData\Abp.Web.Api.OData.csproj" -Properties Configuration=Release -IncludeReferencedProjects -Symbols
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp.Web.Resources\Abp.Web.Resources.csproj" -Properties Configuration=Release -IncludeReferencedProjects -Symbols
"..\src\.nuget\NuGet.exe" "pack" "..\src\TestBase\Abp.TestBase\Abp.TestBase.csproj" -Properties Configuration=Release -IncludeReferencedProjects -Symbols

ren *.symbols.nupkg *.temp_nupkg
del /a /f /q *.nupkg
ren *.temp_nupkg *.nupkg
::Require "Debugging Tools for Windows"
@powershell GitLink.exe %cd%\../ -u https://github.com/aspnetboilerplate/aspnetboilerplate -c release
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp\Abp.csproj" -Properties Configuration=Release -IncludeReferencedProjects 
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp.AutoMapper\Abp.AutoMapper.csproj" -Properties Configuration=Release -IncludeReferencedProjects 
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp.HangFire\Abp.HangFire.csproj" -Properties Configuration=Release -IncludeReferencedProjects 
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp.EntityFramework\Abp.EntityFramework.csproj" -Properties Configuration=Release -IncludeReferencedProjects 
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp.FluentMigrator\Abp.FluentMigrator.csproj" -Properties Configuration=Release -IncludeReferencedProjects 
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp.MemoryDb\Abp.MemoryDb.csproj" -Properties Configuration=Release -IncludeReferencedProjects 
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp.MongoDB\Abp.MongoDB.csproj" -Properties Configuration=Release -IncludeReferencedProjects 
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp.NHibernate\Abp.NHibernate.csproj" -Properties Configuration=Release -IncludeReferencedProjects 
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp.RedisCache\Abp.RedisCache.csproj" -Properties Configuration=Release -IncludeReferencedProjects 
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp.Owin\Abp.Owin.csproj" -Properties Configuration=Release -IncludeReferencedProjects 
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp.Web\Abp.Web.csproj" -Properties Configuration=Release -IncludeReferencedProjects 
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp.Web.Api\Abp.Web.Api.csproj" -Properties Configuration=Release -IncludeReferencedProjects 
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp.Web.Mvc\Abp.Web.Mvc.csproj" -Properties Configuration=Release -IncludeReferencedProjects 
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp.Web.SignalR\Abp.Web.SignalR.csproj" -Properties Configuration=Release -IncludeReferencedProjects 
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp.Web.Api.OData\Abp.Web.Api.OData.csproj" -Properties Configuration=Release -IncludeReferencedProjects 
"..\src\.nuget\NuGet.exe" "pack" "..\src\Abp.Web.Resources\Abp.Web.Resources.csproj" -Properties Configuration=Release -IncludeReferencedProjects 
"..\src\.nuget\NuGet.exe" "pack" "..\src\TestBase\Abp.TestBase\Abp.TestBase.csproj" -Properties Configuration=Release -IncludeReferencedProjects 
pause
