## Note: Use with -s parameter to serve generated docfx project.
## Note: Required docfx to work. See:https://dotnet.github.io/docfx/tutorial/docfx_getting_started.html
# COMMON PATHS

$currentPath = (Get-Item -Path "./" -Verbose).FullName
$docfxProjectPath = Join-Path $currentPath "docfx_project"

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

## CLONE ASPNETBOILERPLATE ######################################################################

"{0}-CLONING ASP.NET BOILERPLATE-" -f [environment]::NewLine

cd docfx_project\src
git clone https://github.com/aspnetboilerplate/aspnetboilerplate.git

"-ASP.NET BOILERPLATE CLONED-{0}" -f [environment]::NewLine

## BUILD ASPNETBOILERPLATE ######################################################################

"{0}-BUILDING ASP.NET BOILERPLATE-"  -f [environment]::NewLine

cd aspnetboilerplate
dotnet restore Abp.sln
dotnet build Abp.sln

"-BUILD COMPLETED-{0}" -f [environment]::NewLine

## CREATE DOCFX FILES ASPNETBOILERPLATE ######################################################################

"{0}-CREATING DOCFX FILES-" -f [environment]::NewLine

cd ../.. 
docfx

"-DOCFX FILES HAS BEEN CREATE-{0}" -f [environment]::NewLine

## SERVE DOCFX PROJECT ######################################################################
"{0}-CREATION HAS BEEN COMPLETED-" -f [environment]::NewLine

if($args[0] -eq "-s"){
    "{0}-SERVING PROJECT-" -f [environment]::NewLine
    cd ..
    docfx docfx_project\docfx.json --serve
}else {
    " Note: "
    "     You can move to the main directory that has 'docfx.json' and run 'docfx docfx_project\docfx.json --serve' command to serve project."
    "     Or just run 'create-docs.ps1' with '-s' parameter to create and serve docfx. ('create-docs.ps1 -s')" 
}