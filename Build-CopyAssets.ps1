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
        [Parameter(Position=0,Mandatory=1)][string]$path,
        [Parameter(Position=1,Mandatory=0)][string]$target
    )
	Copy-Item $path $target
	AssertPath $path
}

function CopyAssets {
	$assetPath = '.\src\ConfigServer.Server\Assets'
	Write-Host "Copying Assets from ConfigServer.Gui to ConfigServer.Server"
	CopyItemWithAssert '.\src\ConfigServer.Gui\ClientApp\dist\runtime.js' $assetPath
	CopyItemWithAssert '.\src\ConfigServer.Gui\ClientApp\dist\polyfills.js' $assetPath
	CopyItemWithAssert '.\src\ConfigServer.Gui\ClientApp\dist\main.js' $assetPath
	CopyItemWithAssert '.\src\ConfigServer.Gui\ClientApp\dist\styles.css' $assetPath

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


