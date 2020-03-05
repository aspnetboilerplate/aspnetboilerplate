$version=$args[0]
[xml]$xml = Get-Content .\common.props
$xml.SelectSingleNode('//Version')."#text"= $version
$xml.Save('.\common.props')