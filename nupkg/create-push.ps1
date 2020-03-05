$prefix = '"..\tools\nuget\NuGet.exe" "push" "'
$suffix = '" API_KEY -Source https://api.nuget.org/v3/index.json'

Get-ChildItem .\*.nupkg | Select-Object { $prefix + $_.Name + $suffix }  | Out-File -width 1000 .\push.bat -Force

(gc .\push.bat | select -Skip 3) | sc .\push.bat




