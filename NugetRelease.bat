@echo off
for /f %%i in (needPublishPro.config) DO (
    D:\Vs2017\MSBuild\15.0\Bin\MSBuild.exe %%i /t:pack /p:Configuration=Release
    move %%i\..\bin\Release\*.nupkg .\nupkg\
)

.\tools\nuget\nuget.exe push .\nupkg\*.nupkg -s http://192.168.19.88:1024/nuget clear
rem del /q /f .\nupkg\*.nupkg

echo 上传packge完成，输入任意键继续
set/p xxxx= >nul