$prefix = '"..\tools\nuget\NuGet.exe" "push" "'
$suffix = '" API_KEY -Source https://api.nuget.org/v3/index.json'

Get-ChildItem .\*.nupkg | Select-Object { $prefix + $_.Name + $suffix }  | Out-File -width 1000 .\push.ps1 -Force

(gc .\push.ps1 | select -Skip 3) | sc .\push.ps1




