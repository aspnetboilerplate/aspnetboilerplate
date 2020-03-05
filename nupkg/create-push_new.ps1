$prefix = '"..\tools\nuget\NuGet.exe" "push" "'
$suffix = '" oy2f2ydj4clad7sgbxsqoqp5rforxz6exnuhkyxd7jduby -Source https://api.nuget.org/v3/index.json'

Get-ChildItem .\*.nupkg | Select-Object { $prefix + $_.Name + $suffix }  | Out-File -width 1000 .\push.bat -Force

(gc .\push.bat | select -Skip 3) | sc .\push.bat




