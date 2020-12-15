$prefix = 'dotnet nuget push '
$suffix = ' --source https://api.nuget.org/v3/index.json --api-key API_KEY'

Get-ChildItem .\*.nupkg | Select-Object { $prefix + $_.Name + $suffix }  | Out-File -width 1000 .\push.bat -Force

(gc .\push.bat | select -Skip 3) | sc .\push.bat




