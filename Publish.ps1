param
(
	[Parameter()]
	[Switch] $Preview
)

$scriptPath = $PSScriptRoot
$files = Get-Item "$scriptPath\Binaries\*.nupkg"

foreach ($file in $files)
{
	Write-Host $file.FullName -ForegroundColor Cyan

	if ($Preview.IsPresent)
	{
		continue
	}
	
	& "nuget.exe" push $file.FullName -Source https://www.nuget.org/api/v2/package
	
	if (Test-Path "C:\Workspaces\Nuget\Release")
	{
		Copy-Item $file.FullName "C:\Workspaces\Nuget\Release\$($file.Name)"
	}
}