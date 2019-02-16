function AssertPath {
	[CmdletBinding()]	
	param(
        [Parameter(Position=0,Mandatory=1)][string]$path
    )
	if( (Test-Path $path) -eq $false) {
		Write-Error "Asset $path not found";
		exit 1;
	}
}

function CopyItemWithAssert {
	[CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=1)][string]$sourceDir,
        [Parameter(Position=1,Mandatory=0)][string]$targetDir,
		[Parameter(Position=2,Mandatory=0)][string]$file
    )
	$path = $sourceDir + "\" + $file
	$targetPath = $targetDir + "\" + $file
	Write-Host "Copy Path:"  $path 
	AssertPath $path
	Copy-Item $path $targetDir
	Write-Host "To Path:"  $targetPath
	AssertPath $targetPath
}

function CopyAssets {
	$assetPath = '.\src\ConfigServer.Server\Assets'
	$sourcePath = '.\src\ConfigServer.Gui\ClientApp\dist'
	Write-Host "Copying Assets from ConfigServer.Gui to ConfigServer.Server"
	CopyItemWithAssert $sourcePath $assetPath 'runtime.js' 
	CopyItemWithAssert $sourcePath $assetPath 'polyfills.js'
	CopyItemWithAssert $sourcePath $assetPath 'main.js'
	CopyItemWithAssert $sourcePath $assetPath 'styles.css'

}

function AssertAssets {
	Write-Host "Checking asset have been generated and copied"
	AssertPath '.\src\ConfigServer.Server\Assets\runtime.js'
	AssertPath '.\src\ConfigServer.Server\Assets\polyfills.js'	
	AssertPath '.\src\ConfigServer.Server\Assets\main.js' 
	AssertPath '.\src\ConfigServer.Server\Assets\styles.css' 
}

CopyAssets
AssertAssets


