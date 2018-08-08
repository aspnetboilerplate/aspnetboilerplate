# 设置输出格式
$OutputEncoding = [Text.UTF8Encoding]::UTF8

## 公共参数

# 替换前的名称
$oldName="MyCompanyName.AbpZeroTemplate"

# 替换后的名称
$newName="Zrd.AbpDemo"

$oldProjectName="AbpZeroTemplate"
$newProjectName="AbpDemo"

# 文件类型名称
$fileType="FileInfo"

# 目录类型名称
$dirType="DirectoryInfo"

# sln所在目录
$slnFolder = (Get-Item -Path "./aspnet-core/" -Verbose).FullName
$angluarFolder = (Get-Item -Path "./angular/" -Verbose).FullName

# 需要修改文件内容的文件后缀名
$include=@("*.cs","*.csproj","*.sln","Dockerfile","*.ps1","*.ts","*.cshtml","*.asax")

## 替换文件内容
# 替换文件中的内容替换
Write-Host '开始替换文件内容'
Ls $slnFolder -Include $include -Recurse | ForEach-Object{
    (Get-Content $_) -replace $oldName,$newName | Set-Content $_
}
Write-Host '结束替换文件内容'

Write-Host '开始重命名文件'
# 重命名文件
Ls $slnFolder -Recurse | Where { $_.GetType().Name -eq $fileType -and $_.Name.Contains($oldName) } | ForEach-Object{
	Write-Host 'file ' $_.Name
	$newFileName=$_.Name.Replace($oldName,$newName)   
	Rename-Item $_.FullName $newFileName
}
Write-Host '结束重命名文件'

Write-Host '开始重命名文件夹'
# 重命名文件夹
Ls $slnFolder -Recurse | Where { $_.GetType().Name -eq $dirType -and $_.Name.Contains($oldName) } | ForEach-Object{
	Write-Host 'directory ' $_.Name
	$newDirectoryName=$_.Name.Replace($oldName,$newName)   
	Rename-Item $_.FullName $newDirectoryName
}
Write-Host '结束重命名文件夹'

Write-Host '开始替换文件中的项目名'
Ls $slnFolder -Include $include -Recurse | ForEach-Object{
    (Get-Content $_) -replace $oldProjectName,$newProjectName | Set-Content $_
}
Write-Host '结束替换文件中的项目名'

Write-Host '开始替换文件名中的项目名'
Ls $slnFolder -Recurse | Where { $_.GetType().Name -eq $fileType -and $_.Name.Contains($oldProjectName) }  | ForEach-Object{
	Write-Host 'file ' $_.Name
	$newFileName=$_.Name.Replace($oldProjectName,$newProjectName)   
	Rename-Item $_.FullName $newFileName
}
Write-Host '结束替换文件名中的项目名'

# 替换angular项目文件内容
Ls $angluarFolder -Include $include -Recurse | ForEach-Object{
	Write-Host 'file ' $_.Name
	(Get-Content $_) -replace $oldName,$newName | Set-Content $_
	(Get-Content $_) -replace $oldProjectName,$newProjectName | Set-Content $_
}

# 重命名angular项目文件名
Ls $angluarFolder -Recurse | Where { $_.GetType().Name -eq $fileType -and $_.Name.Contains($oldName) }  | ForEach-Object{
	Write-Host 'file ' $_.Name
	$newFileName=$_.Name.Replace($oldName,$newName)   
	Rename-Item $_.FullName $newFileName
}
