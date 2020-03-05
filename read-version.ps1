[xml]$xml = Get-Content .\common.props
Write-Host $xml.SelectSingleNode('//Version')."#text"
