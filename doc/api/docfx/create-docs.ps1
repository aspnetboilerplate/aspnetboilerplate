## Note: Use with -s parameter to serve generated docfx project.
## Note: Required docfx to work. See:https://dotnet.github.io/docfx/tutorial/docfx_getting_started.html
# COMMON PATHS

$currentPath = (Get-Item -Path "./" -Verbose).FullName
$docfxProjectPath = Join-Path $currentPath "docfx_project"

$indexTocYmlPath = Join-Path $docfxProjectPath "toc.yml"
$indexTocYmlOverridedPath = Join-Path $currentPath "overrides/toc.yml"

$indexMdPath = Join-Path $docfxProjectPath "index.md"
$apiIndexMdPath = Join-Path $docfxProjectPath "api/index.md"
$indexMdOverridedPath = Join-Path $currentPath "overrides/index.md"

$faviconPath = Join-Path $docfxProjectPath "images/favicon.ico"
$faviconOverridedPath = Join-Path $currentPath "overrides/favicon.ico"

$logoPath = Join-Path $docfxProjectPath "images/logo.png"
$logoOverridedPath = Join-Path $currentPath "overrides/logo.png"


$docFxJsonFilePath = Join-Path $docfxProjectPath "docfx.json"
$docFxJsonFileOverridedPath = Join-Path $currentPath "overrides/docfx.json"

$templateFolderPath = Join-Path $docfxProjectPath "template"
$templateFolderOverridedPath = Join-Path $currentPath "overrides/template"

## CLEAR ######################################################################

"{0}-CLEARING CURRENT API DOC FILES-" -f [environment]::NewLine

If (Test-path $docfxProjectPath) {
    Remove-Item $docfxProjectPath -Recurse -Force -confirm:$false
}

"-API DOCSFX FILES HAS BEEN CLEANED-{0}" -f [environment]::NewLine

## INIT DOCFX ######################################################################

"{0}-INITIALIZING EMPTY DOCFX PROJECT-" -f [environment]::NewLine

docfx init -q

"-INITIALIZATION COMPLETED-{0}" -f [environment]::NewLine

## REPLACING OVERRIDE ITEMS ######################################################################

"{0}-REPLACING OVERRIDE ITEMS-" -f [environment]::NewLine

If (Test-path $indexTocYmlPath) {
    Remove-Item $indexTocYmlPath -Force -ErrorAction Ignore
}
Copy-Item -Path $indexTocYmlOverridedPath -Destination $indexTocYmlPath

If (Test-path $indexMdPath) {
    Remove-Item $indexMdPath -Force -ErrorAction Ignore
}
Copy-Item -Path $indexMdOverridedPath -Destination $indexMdPath

If (Test-path $apiIndexMdPath) {
    Remove-Item $apiIndexMdPath -Force -ErrorAction Ignore
}
Copy-Item -Path $indexMdOverridedPath -Destination $apiIndexMdPath

If (Test-path $logoPath) {
    Remove-Item $logoPath -Force -ErrorAction Ignore
}
Copy-Item -Path $logoOverridedPath -Destination $logoPath

If (Test-path $faviconPath) {
    Remove-Item $faviconPath -Force -ErrorAction Ignore
}
Copy-Item -Path $faviconOverridedPath -Destination $faviconPath

If (Test-path $docFxJsonFilePath) {
    Remove-Item $docFxJsonFilePath -Force -ErrorAction Ignore
}
Copy-Item -Path $docFxJsonFileOverridedPath -Destination $docFxJsonFilePath

Copy-Item -Path $templateFolderOverridedPath -Destination $templateFolderPath -Recurse -Force

"-ITEMS REPLACED-{0}" -f [environment]::NewLine

## CLONE ASPNETBOILERPLATE ######################################################################

"{0}-CLONING ASP.NET BOILERPLATE-" -f [environment]::NewLine

cd docfx_project\src
git clone https://github.com/aspnetboilerplate/aspnetboilerplate.git

"-ASP.NET BOILERPLATE CLONED-{0}" -f [environment]::NewLine

## BUILD ASPNETBOILERPLATE ######################################################################

"{0}-BUILDING ASP.NET BOILERPLATE-"  -f [environment]::NewLine

cd aspnetboilerplate
dotnet restore Abp.sln
dotnet build Abp.sln -c Release

"-BUILD COMPLETED-{0}" -f [environment]::NewLine

## CREATE DOCFX FILES ASPNETBOILERPLATE ######################################################################

"{0}-CREATING DOCFX FILES-" -f [environment]::NewLine

cd ../.. 
docfx

"-DOCFX FILES HAS BEEN CREATE-{0}" -f [environment]::NewLine

## SERVE DOCFX PROJECT ######################################################################
"{0}-CREATION HAS BEEN COMPLETED-" -f [environment]::NewLine
cd ..

if($args[0] -eq "-s"){
    "{0}-SERVING PROJECT-" -f [environment]::NewLine
    docfx docfx_project\docfx.json --serve
}else {
    " Note: "
    "     You can move to the main directory that has 'docfx.json' and run 'docfx docfx_project\docfx.json --serve' command to serve project."
    "     Or just run 'create-docs.ps1' with '-s' parameter to create and serve docfx. ('create-docs.ps1 -s')" 
}